using eJournal.Domain.Models;
using eJournal.Services.Interfaces;
using eJournal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eJournal.Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly INotificationService _notificationService;
        private readonly ILikeService _likeService;

        public CommentController(
            ICommentService commentService,
            IUserService userService,
            IImageService imageService,
            INotificationService notificationService,
            ILikeService likeService
            )
        {
            _commentService = commentService;
            _userService = userService;
            _imageService = imageService;
            _notificationService = notificationService;
            _likeService = likeService;
        }

        public async Task<IActionResult> GetAllCommentsByBlogId(int blogId)
        {
            var result = _commentService.GetCommentsByBlogId(blogId).ToListAsync().Result.ToList();
            result = result.OrderBy(comment => comment.CreatedAt).ToList();

            List<CommentViewModel> resultList = await PopulateCommentViewModel(result);

            return Ok(resultList);
        }
        public async Task<IActionResult> GetAllCommentsByCommentId(int commentId)
        {
            var result = _commentService.GetCommentsByCommentId(commentId).ToListAsync().Result.ToList();
            result = result.OrderBy(comment => comment.CreatedAt).ToList();

            List<CommentViewModel> resultList = await PopulateCommentViewModel(result);

            return Ok(resultList);
        }

        public async Task<IActionResult> Create()
        {
            int userId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);
            User user = await _userService.GetUserById(userId);
            try
            {
                Image image = await _imageService.GetImageByIdAsync((int)user.ImageId);
                ViewBag.UserImage = image.ImagePath;
            }
            catch (Exception ex)
            {
                ViewBag.UserImage = "Images\\user.png";
            }
            return PartialView("_CreateCommentPartial");
        }

        [HttpPost]
        public async Task<IActionResult> Create(CommentCreateViewModel commentViewModel, int commentId = 0, int blogId = 0)
        {
            if (ModelState.IsValid)
            {
                Comment comment = new()
                {
                    CommentText = commentViewModel.CommentText,
                    UserId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value),
                    CreatedAt = DateTime.Now
                };

                if (blogId > 0)
                {
                    comment.BlogId = blogId;
                }
                else if (commentId > 0)
                {
                    comment.ParentCommentId = commentId;
                }
                else
                {
                    return BadRequest("No BlogId or ParentCommentId has been provided");
                }

                try
                {
                    var savedComment = await _commentService.CreateCommentAsync(comment);
                    CommentViewModel result = new()
                    {
                        CommentText = savedComment.CommentText,
                        CreatedAt = savedComment.CreatedAt,
                        CommentId = (int)savedComment.CommentId,
                        UserId = (int)savedComment.UserId
                    };
                    result.LoggedInUserId = result.UserId;

                    User user = await _userService.GetUserById((int)comment.UserId);
                    result.UserName = user.UserName;
                    try
                    {
                        Image image = await _imageService.GetImageByIdAsync((int)user.ImageId);
                        result.UserImage = image.ImagePath;
                    }
                    catch (Exception ex)
                    {
                        result.UserImage = "Images\\user.png";
                    }

                    if (savedComment.BlogId != null)
                    {
                        await _notificationService.GenerateCommentNotification((long)savedComment.BlogId, savedComment.UserId);
                    }
                    else
                    {
                        await _notificationService.GenerateNotificationForReplyInComment((long)savedComment.CommentId, savedComment.UserId);
                    }
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest("Couldn't save the Comment: " + ex);
                }
            }
            else
            {
                return BadRequest("No text has been provided");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IFormCollection formCollection)
        {
            int commentId = int.Parse(formCollection["commentId"]);
            string commentText = formCollection["modifiedText"];
            Comment comment = await _commentService.GetCommentByIdAsync(commentId);
            if (comment != null)
            {
                comment.UpdatedAt= DateTime.Now;
                comment.CommentText = commentText;
                var updatedComment = await _commentService.UpdateCommentAsync(comment);
                return Ok(updatedComment);
            }
            return BadRequest("Did not found the comment to update");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int commentId)
        {
            try
            {
                await _commentService.DeleteCommentAsync(commentId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Could not delete the comment" + ex.Message);
            }
        }

        private async Task<List<CommentViewModel>> PopulateCommentViewModel(List<Comment> result)
        {
            List<CommentViewModel> resultList = new List<CommentViewModel>();

            foreach (var comment in result)
            {
                CommentViewModel commentViewModel = new CommentViewModel()
                {
                    CommentId = (int)comment.CommentId,
                    UserId = (int)comment.UserId,
                    CommentText = comment.CommentText,
                    CreatedAt = comment.CreatedAt,
                    LoggedInUserId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value),
                    TotalLikes = _likeService.GetAllLikesByCommentId((int)comment.CommentId).ToListAsync().Result.Count()
                };

                Like like = _likeService.GetLikeByUserAndCommentId(commentViewModel.LoggedInUserId, (int)comment.CommentId);
                if (like != null)
                {
                    commentViewModel.IsLikedByMe = true;
                }
                else
                {
                    commentViewModel.IsLikedByMe = false;
                }

                User user = await _userService.GetUserById((int)comment.UserId);
                commentViewModel.UserName = user.UserName;
                try
                {
                    Image image = await _imageService.GetImageByIdAsync((int)user.ImageId);
                    commentViewModel.UserImage = image.ImagePath;
                }
                catch (Exception ex)
                {
                    commentViewModel.UserImage = "Images\\user.png";
                }
                resultList.Add(commentViewModel);
            }
            return resultList;
        }
    }
}
