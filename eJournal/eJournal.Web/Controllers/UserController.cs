using eJournal.Domain.Models;
using eJournal.Services.Interfaces;
using eJournal.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace eJournal.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly string rootPath;
        private readonly string imageRootPath;
        public UserController(IUserService userService, IImageService imageService, IWebHostEnvironment webHostEnvironment)
        {
            _userService = userService;
            _imageService = imageService;
            _webHostEnvironment = webHostEnvironment;
            rootPath = _webHostEnvironment.WebRootPath;
            imageRootPath = Path.Combine(rootPath, "Images\\UserImages");
        }

        [HttpGet]
        public async Task<ActionResult> Profile(long blogUserId)
        {
            ClaimsPrincipal principal = HttpContext.User as ClaimsPrincipal;
            string currentUserId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userImageUrl = Request.Cookies["ImageUrl"];
            User user = new User();
            if (blogUserId != 0)
            {
                user = await _userService.GetUserById((int)blogUserId);
            }
            else
            {
                int id = int.Parse(currentUserId);
                user = await _userService.GetUserById(id);
            }
            if (user != null)
            {
                if (user.ImageId == null)
                {
                    var image = new Image();
                    image.ImagePath = userImageUrl;
                    user.Image = image;
                }
                else
                {
                    Image img = await _imageService.GetImageByIdAsync((long)user.ImageId);
                    user.Image = img;
                }
                return View(user);
            }
            else
            {
                User newuser = new User();
                newuser.UserName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                newuser.UserEmail = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                newuser.FirstName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
                newuser.LastName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
                newuser.DateOfBirth =Convert.ToDateTime(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.DateOfBirth)?.Value);
                newuser.Phone =  principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value;
                newuser.Gender =  principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Gender)?.Value;
                var image = new Image();
                image.ImagePath = userImageUrl;
                newuser.Image = image;
                return View(newuser);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(UserViewModel model)
        {
            ClaimsPrincipal principal = HttpContext.User as ClaimsPrincipal;
            string currentUserId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            string currentUserEmail = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var userImageUrl = Request.Cookies["ImageUrl"];
            int id = int.Parse(currentUserId);
            User user = await _userService.GetUserById(id);

            if (user != null)
            {
                model.UserId = user.UserId;
                model.UserName = user.UserName;
                model.UserEmail = user.UserEmail;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Bio = user.Bio;
                model.Gender = user.Gender;
                model.Phone = user.Phone;
                model.DateOfBirth = user.DateOfBirth;
                model.Designation = user.Designation;
                model.Department = user.Department;
                model.IsActive = user.IsActive;
                if (user.ImageId == null)
                {
                    model.ImagePath = null;
                }
                else
                {
                    Image img = await _imageService.GetImageByIdAsync((long)user.ImageId);
                    model.ImagePath=img.ImagePath;
                    model.ImageId=img.ImageId;
                }
            }
            else
            {
                model.ImagePath = null;
                model.UserId = 0;
                model.DateOfBirth = DateTime.Now.Date;
                model.UserEmail = currentUserEmail;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(IFormCollection formcollection)
        {
            var savedImage = new Image();
            User user = new User();
            user.UserId = (long)Convert.ToUInt64(formcollection["UserId"]);
            user.UserName = formcollection["UserName"];
            user.FirstName = formcollection["FirstName"];
            user.LastName = formcollection["LastName"];
            user.Designation = formcollection["Designation"];
            user.Department = formcollection["Department"];
            user.DateOfBirth = Convert.ToDateTime(formcollection["DateOfBirth"]);
            user.Gender = formcollection["Gender"];
            user.IsActive =  Convert.ToBoolean(formcollection["isactive"]);
            user.IsActive.GetType();
            user.Phone = formcollection["Phone"];
            user.UserEmail = formcollection["UserEmail"];
            user.Bio = formcollection["Bio"];
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            var viewImageId = Convert.ToInt64(formcollection["ImageId"]);
            var viewImagePath = formcollection["ImagePath"];

            ClaimsPrincipal principal = HttpContext.User as ClaimsPrincipal;
            string currentUserEmail = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            user.UserEmail = currentUserEmail;

            bool newUser = false;
            if (user.UserId == 0)
            {
                newUser=true;
            }


            bool errorExist = false;
            if (user.UserName == "")
            {
                ViewBag.UserNameErrorMessage = "Please Enter the User Name";
                errorExist = true;
            }
            if (user.UserName != null)
            {
                bool isUserNameExist = await _userService.IfUserNameExist(user.UserName, (int)user.UserId);
                if (isUserNameExist == true)
                {
                    ViewBag.UserNameErrorMessage = "Sorry! You Are Entering A Duplicate User Name";
                    errorExist = true;
                }
            }
            if (user.FirstName == "")
            {
                ViewBag.FirstNameErrorMessage = "Please Enter your First Name";
                errorExist = true;
            }
            if (user.LastName == "")
            {
                ViewBag.LastNameErrorMessage = "Please Enter your Last Name";
                errorExist = true;
            }
            if (user.Gender == "")
            {
                ViewBag.GenderErrorMessage = "Please Enter your Gender";
                errorExist = true;
            }
            if (user.Department == "")
            {
                ViewBag.DepartmentErrorMessage = "Please Enter your Department ";
                errorExist = true;
            }
            if (user.Designation == "")
            {
                ViewBag.DesignationErrorMessage = "Please Enter your Designation ";
                errorExist = true;
            }
            if (user.DateOfBirth!=null)
            {
                if (user.DateOfBirth.Date > DateTime.Today.Date)
                {
                    ViewBag.DateOfBirthErrorMessage = "You are adding a wrong birthday";
                    errorExist = true;
                }
            }
            if (user.Phone == "")
            {
                ViewBag.PhoneErrorMessage = "Please Enter your Phone Number ";
                errorExist = true;
            }
            if (user.Phone != "")
            {
                string pattern = @"^[0-9]+$";
                bool isMatch = Regex.IsMatch(user.Phone, pattern);
                if (isMatch)
                {
                    ViewBag.PhoneValidErrorMessage="";
                }
                else
                {
                    ViewBag.PhoneValidErrorMessage = "Invalid Phone Number";
                    errorExist = true;
                }
            }
            if (errorExist==true)
            {
                UserViewModel model = new UserViewModel();
                model.UserId = user.UserId;
                model.UserName=user.UserName;
                model.UserEmail=user.UserEmail;
                model.FirstName=user.FirstName;
                model.LastName=user.LastName;
                model.Bio=user.Bio;
                model.Gender=user.Gender;
                model.Phone=user.Phone;
                model.DateOfBirth=user.DateOfBirth;
                model.Designation=user.Designation;
                model.Department=user.Department;
                model.IsActive=user.IsActive;
                return View(model);
            }


            else
            {
                if (formcollection.Files.Count>0)
                {
                    if (viewImageId!=0)
                    {
                        user.ImageId=null;
                        await _userService.UpdateUser(user);
                        await DeleteImage(viewImageId);
                    }
                    savedImage = await CreateImage(formcollection.Files[0]);
                    user.ImageId=savedImage.ImageId;
                }
                else
                {
                    if (viewImagePath != "")
                    {
                        user.ImageId = viewImageId;
                    }
                    else
                    {
                        user.ImageId = null;
                        await _userService.UpdateUser(user);
                        if (viewImageId != 0)
                        {
                            await DeleteImage(viewImageId);
                        }
                    }
                }

                if (ModelState.IsValid)
                {

                    await _userService.UpdateUser(user);
                    User UpdatedUser = await _userService.GetUserByEmail(user.UserEmail);
                    if (UpdatedUser != null)
                    {
                        var userIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                        userIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, UpdatedUser.UserId.ToString()));
                        userIdentity.AddClaim(new Claim(ClaimTypes.Name, UpdatedUser.UserName.ToString()));
                        userIdentity.AddClaim(new Claim(ClaimTypes.Email, UpdatedUser.UserEmail.ToString()));
                        userIdentity.AddClaim(new Claim("UserName", UpdatedUser.UserName.ToString()));

                        var userPrincipal = new ClaimsPrincipal(userIdentity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
                    }

                    if (newUser == true)
                    {

                        User savedUser = await _userService.GetUserByEmail(user.UserEmail);
                        var userIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                        userIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, savedUser.UserId.ToString()));
                        userIdentity.AddClaim(new Claim(ClaimTypes.Name, savedUser.UserName.ToString()));
                        userIdentity.AddClaim(new Claim(ClaimTypes.Email, savedUser.UserEmail.ToString()));
                        userIdentity.AddClaim(new Claim("UserName", savedUser.UserName.ToString()));

                        var userPrincipal = new ClaimsPrincipal(userIdentity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                    }
                    return RedirectToAction("Profile", "User");
                }
                return View();
            }
        }
        private async Task<Image> CreateImage(IFormFile formFile)
        {
            Image image = new();
            image.ImageName = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss") + "_" + formFile.FileName;
            image.ImagePath = Path.Combine("Images\\UserImages", image.ImageName);
            var imageSavePath = Path.Combine(imageRootPath, image.ImageName);
            var file = formFile;
            try
            {
                using (var stream = System.IO.File.Create(imageSavePath))
                {
                    await file.CopyToAsync(stream);
                }
                var savedImage = await _imageService.CreateImageAsync(image);
                return savedImage;
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't Create the image" + ex);
            }
        }

        private async Task DeleteImage(long viewImageId)
        {
            Image existingImage = await _imageService.GetImageByIdAsync(viewImageId);
            string[] splitPath = existingImage.ImagePath.Split(new string[] { "UserImages\\" }, StringSplitOptions.None);
            string splitImagePath = splitPath[1];
            var imageDeletePath = Path.Combine(imageRootPath, splitImagePath);
            try
            {
                if (existingImage != null)
                {
                    System.IO.File.Delete(imageDeletePath);
                    await _imageService.DeleteImageAsync(viewImageId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't delete the image" + ex);
            }
        }

    }

}