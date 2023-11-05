using eJournal.Domain.Models;
using eJournal.Services.Interfaces;
using eJournal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace eJournal.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogService _blogService;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;
        private readonly INotificationService _notificationService;
        private int BlogsPerPage;
        private string RootPath;
        public HomeController(
            ILogger<HomeController> logger,
            IBlogService blogService,
            IUserService userService,
            IImageService imageService,
            IConfiguration configuration,
            ILikeService likeService,
            ICommentService commentService,
            INotificationService notificationService,
            IWebHostEnvironment webHostEnvironment
            )
        {
            _logger = logger;
            _blogService = blogService;
            _userService = userService;
            _imageService = imageService;
            _webHostEnvironment = webHostEnvironment;
            _commentService = commentService;
            _likeService = likeService;
            _notificationService = notificationService;
            RootPath = _webHostEnvironment.WebRootPath;
            BlogsPerPage = int.Parse(configuration.GetSection("AppConfiguration")["BlogPerPage"] ?? "10");
        }
        public async Task<IActionResult> Index(bool filterMyCreations, int page = 1, string searchtext = null, int blogId = 0)
        {
            string text = searchtext;
            List<Blog> allBlogs;
            int totalBlogsCount = 0;
            int skip = (page - 1) * BlogsPerPage;
            if (filterMyCreations)
            {
                int userId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);
                ViewBag.FilterMyCreations = "checked";
                totalBlogsCount = await _blogService.GetAllBlogsByUserId(userId).CountAsync();
                allBlogs = _blogService.GetAllBlogsByUserId(userId, skip, BlogsPerPage).ToListAsync().Result.ToList();
            }
            else if (blogId != 0)
            {
                totalBlogsCount = 1;
                allBlogs = new List<Blog>();
                Blog blog = await _blogService.GetBlogByIdAsync(blogId);
                allBlogs.Add(blog);
            }
            else
            {

                if (!string.IsNullOrEmpty(text))
                {
                    totalBlogsCount = await _blogService.SearchBlogsCountAsync(text);
                    allBlogs = await _blogService.SearchBlogsAsync(text, skip, BlogsPerPage).Result.ToListAsync();
                    allBlogs = allBlogs.OrderByDescending(blog => blog.CreatedAt).ToList();
                    ViewBag.SearchText = searchtext;
                }
                else
                {
                    totalBlogsCount = await _blogService.GetAllBlogsAsync().Result.CountAsync();
                    allBlogs = await _blogService.GetAllBlogsAsync(skip, BlogsPerPage).Result.ToListAsync();
                    allBlogs = allBlogs.OrderByDescending(blog => blog.CreatedAt).ToList();
                }
            }

            Pagination pagination = new Pagination(totalBlogsCount, BlogsPerPage, page);
            ViewBag.Pagination = pagination;

            int _userId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);
            User user = await _userService.GetUserById(_userId);

            try
            {
                Image image = await _imageService.GetImageByIdAsync((int)user.ImageId);
                ViewBag.LoggedInUserImage = image.ImagePath;
            }
            catch (Exception ex)
            {
                ViewBag.LoggedInUserImage = "Images\\user.png";
            }

            List<BlogViewModel> allBlogsViewModel = await PopulateBlogViewModel(allBlogs);
            return View(allBlogsViewModel);
        }


        public async Task<IActionResult> TopRecentPosts()
        {
            var topPosts = await _blogService.GetAllBlogsAsync(take: 5);
            var blogViewModelList = new List<BlogViewModel>();

            await foreach (var post in topPosts)
            {
                var blogViewModel = new BlogViewModel
                {
                    BlogTitle = post.BlogTitle,
                    BlogText = post.BlogText,
                    BlogId = post.BlogId
                };

                blogViewModelList.Add(blogViewModel);
            }

            return PartialView("_TopPosts", blogViewModelList);
        }
        public async Task<IActionResult> TopPopularPosts()
        {
            List<int> TopFiveBlogId = await _likeService.GetTopFiveBlogIdsWithMostLikes();
            var blogViewModelList = new List<BlogViewModel>();
            foreach (var id in TopFiveBlogId)
            {
                var post = await _blogService.GetBlogByIdAsync(id);
                var blogViewModel = new BlogViewModel
                {
                    BlogTitle = post.BlogTitle,
                    BlogText = post.BlogText,
                    BlogId = post.BlogId
                };

                blogViewModelList.Add(blogViewModel);
            }

            return PartialView("_TopPopularPosts", blogViewModelList);
        }
        public async Task<IActionResult> GetAllNotifications()
        {
            int userid = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);
            var notifications = await _notificationService.GetNotificationsByUserId(userid).ToListAsync();
            return PartialView("_NotificationPartialView", notifications);
        }

        public async Task<IActionResult> MarkNotificationAsChecked(int notificationId)
        {
            if (notificationId == 0)
            {
                int userid = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);
                var notifications = await _notificationService.GetNotificationsByUserId(userid).ToListAsync();
                foreach (var noti in notifications)
                {
                    noti.IsChecked = true;
                    await _notificationService.UpdateNotificationAsync(noti);
                }
                return RedirectToAction("Index");
            }
            else
            {
                var notification = await _notificationService.GetNotificationById(notificationId);
                if (notification != null)
                {
                    notification.IsChecked = true;
                    await _notificationService.UpdateNotificationAsync(notification);
                }
                int notficationBlogId = (int)notification.BlogId;
                return RedirectToAction("Index", "Home", new { blogId = notficationBlogId });
            }
        }
        public IActionResult Create()
        {
            return PartialView("_CreateModalPartial");
        }
        [HttpPost]
        public async Task<IActionResult> Create(BlogCreateViewModel blogCreateViewModel)
        {
            // Ready the blog but don't save it
            Blog blog = new Blog();
            blog.BlogTitle = blogCreateViewModel.BlogTitle;
            blog.BlogText = blogCreateViewModel.BlogText;
            blog.CreatedAt = DateTime.Now;
            blog.UserId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);

            // Ready the image objects for saving in future. Save the files.
            List<Image> images = new List<Image>();
            if (blogCreateViewModel.Images != null)
            {
                foreach (IFormFile formFile in blogCreateViewModel.Images)
                {
                    Image image = new Image();
                    image.ImageName = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss") + "_" + formFile.FileName;
                    image.ImagePath = Path.Combine("Images", "BlogImages", image.ImageName);
                    string placeHolder = String.Format("[#image_{0}]", formFile.FileName);
                    string replaceWith = String.Format("<img class=\"blog-image\" src=\"{0}\" />", image.ImagePath);
                    blog.BlogText = blog.BlogText.Replace(placeHolder, replaceWith);
                    images.Add(image);

                    // save the file in the drive
                    string imageSaveDrivePath = Path.Combine(RootPath, image.ImagePath);
                    try
                    {
                        using (Stream stream = System.IO.File.Create(imageSaveDrivePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Couldn't save the image in drive" + ex);
                    }
                }
            }
            // save the blog and get the id to store with the image
            blog = await _blogService.CreateBlogAsync(blog);
            await _notificationService.CreateNotificationAsync(blog.BlogId, blog.UserId);
            //now save the images in the database with blog_id;
            foreach (Image image in images)
            {
                image.BlogId = blog.BlogId;
                await _imageService.CreateImageAsync(image);
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int blogId)
        {
            var blog = await _blogService.GetBlogByIdAsync(blogId);
            var viewModel = new BlogUpdateViewModel
            {
                BlogTitle = blog.BlogTitle,
                BlogText = blog.BlogText,
                BlogId = (int)blog.BlogId,
            };
            return PartialView("_UpdateModalPartial", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(BlogUpdateViewModel blogUpdateViewModel)
        {
            // Get the blog by id
            var blog = await _blogService.GetBlogByIdAsync((int)blogUpdateViewModel.BlogId);

            // Update the blog properties
            blog.BlogTitle = blogUpdateViewModel.BlogTitle;
            blog.BlogText = blogUpdateViewModel.BlogText;
            blog.UpdatedAt = DateTime.Now;
            blog.BlogId= blogUpdateViewModel.BlogId;

            List<Image> images = new List<Image>();
            if (blogUpdateViewModel.Images != null)
            {
                await DeleteBlogImage(blog.BlogId);

                foreach (IFormFile formFile in blogUpdateViewModel.Images)
                {
                    Image image = new Image();
                    image.ImageName = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss") + "_" + formFile.FileName;
                    image.ImagePath = Path.Combine("Images", "BlogImages", image.ImageName);
                    string placeHolder = String.Format("[#image_{0}]", formFile.FileName);
                    string replaceWith = String.Format("<img class=\"blog-image\" src=\"{0}\" />", image.ImagePath);
                    blog.BlogText = blog.BlogText.Replace(placeHolder, replaceWith);
                    images.Add(image);

                    // save the file in the drive
                    string imageSaveDrivePath = Path.Combine(RootPath, image.ImagePath);
                    try
                    {
                        using (Stream stream = System.IO.File.Create(imageSaveDrivePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Couldn't save the image in drive" + ex);
                    }
                }
            }
            // save the blog and get the id to store with the image
            await _blogService.UpdateBlogAsync(blog);

            //now save the images in the database with blog_id;
            foreach (Image image in images)
            {
                image.BlogId = blog.BlogId;
                await _imageService.CreateImageAsync(image);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int blogId)
        {
            await DeleteBlogImage(blogId);
            await DeleteBlogNotification(blogId);
            var deleteAbleBlogPost = await _blogService.GetBlogByIdAsync(blogId);
            if (deleteAbleBlogPost != null)
            {
                await _blogService.DeleteBlogAsync((int)deleteAbleBlogPost.BlogId);
            }
            return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<List<BlogViewModel>> PopulateBlogViewModel(List<Blog> allBlogs)
        {
            List<BlogViewModel> allBlogsViewModel = new List<BlogViewModel>();
            foreach (var blog in allBlogs)
            {
                BlogViewModel blogViewModel = new BlogViewModel();
                blogViewModel.BlogTitle = blog.BlogTitle;
                blogViewModel.BlogText = blog.BlogText;
                blogViewModel.BlogId = blog.BlogId;
                blogViewModel.UserId = blog.UserId;
                blogViewModel.CreatedAt = blog.CreatedAt;
                blogViewModel.TotalLikes = await _likeService.GetAllLikesByBlogId((int)blog.BlogId).CountAsync();
                blogViewModel.TotalComments = await _commentService.GetCommentsByBlogId((int)blog.BlogId).CountAsync();

                int loggedInUserId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);

                Like like = _likeService.GetLikeByUserAndBlogId(loggedInUserId, (int)blog.BlogId);
                if (like != null)
                {
                    blogViewModel.IsLikedByMe = true;
                }
                else
                {
                    blogViewModel.IsLikedByMe = false;
                }

                User user = await _userService.GetUserById((int)blog.UserId);
                blogViewModel.UserName = user.UserName;

                Image image;
                try
                {
                    image = await _imageService.GetImageByIdAsync((int)user.ImageId);
                }
                catch (Exception ex)
                {
                    image = new();
                    image.ImagePath = "Images\\user.png";
                }
                blogViewModel.UserImage = image.ImagePath;

                allBlogsViewModel.Add(blogViewModel);
            }
            return allBlogsViewModel;
        }
        private async Task DeleteBlogImage(long blogId)
        {
            List<Image> image = await _imageService.GetImageByBlogIdAsync(blogId);
            foreach (Image imageItem in image)
            {
                var imageDeletePath = Path.Combine(RootPath, imageItem.ImagePath);
                try
                {
                    if (imageItem != null)
                    {
                        System.IO.File.Delete(imageDeletePath);
                        await _imageService.DeleteImageAsync(imageItem.ImageId);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Couldn't delete the image" + ex);
                }
            }
        }
        private async Task DeleteBlogNotification(int blogId)
        {
            var notifications = _notificationService.GetNotificationsByBlogId(blogId).ToListAsync().Result.ToList();
            foreach (var notification in notifications)
            {
                try
                {
                    if (notification != null)
                    {
                        await _notificationService.DeleteNotificationAsync((int)notification.NotificationId);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Couldn't delete the image" + ex);
                }
            }
        }


    }
}
