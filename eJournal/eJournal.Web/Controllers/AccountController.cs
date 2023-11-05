using eJournal.Domain.Models;
using eJournal.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eJournal.Web.Controllers
{
    [AllowAnonymous, Route("account")]
    public class AccountController : Controller
    {

        private const string OrganizationDomain = "Your Organizational Domain";
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        [Route("google-login")]
        public IActionResult Index()
        {

            var userEmail = Request.Cookies["UserEmail"];
            var userImageUrl = Request.Cookies["ImageUrl"];
            var username = Request.Cookies["UsernName"];

            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse"),
                Parameters =
                {
                    { "include_granted_scopes", "true" },
                    { "login_hint", User.Identity.IsAuthenticated? userEmail : null },
                    { "image_url", User.Identity.IsAuthenticated? userImageUrl : null },
                    { "username", User.Identity.IsAuthenticated? username : null },

                }
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);


        }

        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault()
                .Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value,
                });

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var imageUrl = claims.FirstOrDefault(c => c.Type == "Image_url")?.Value;

            HttpContext.Response.Cookies.Append("UserEmail", email);
            HttpContext.Response.Cookies.Append("UsernName", username);
            HttpContext.Response.Cookies.Append("ImageUrl", imageUrl);

            TempData["LastUserEmail"] = email;
            TempData["LastUsername"] = username;
            TempData["LastUserImage"] = imageUrl;
            if (IsOrganizationalEmail(email, OrganizationDomain))
            {
                User user = await _userService.GetUserByEmail(email);
                if (user != null)
                {
                    var userId = user.UserId;
                    var userIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    userIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
                    userIdentity.AddClaim(new Claim("UserName", user.UserName.ToString()));

                    foreach (var claim in claims)
                    {
                        userIdentity.AddClaim(new Claim(claim.Type, claim.Value));
                    }

                    var userPrincipal = new ClaimsPrincipal(userIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
                }
                else if (user==null)
                {
                    var userId = 0;
                    var userIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    userIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

                    foreach (var claim in claims)
                    {
                        userIdentity.AddClaim(new Claim(claim.Type, claim.Value));
                    }

                    var userPrincipal = new ClaimsPrincipal(userIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                    return RedirectToAction("Edit", "User");
                }

            }
            else
            {
                return RedirectToAction("Logout", "Account");
            }

            return RedirectToAction("index", "Home");

        }

        [AllowAnonymous]
        [Route("google-logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("login", "account");
        }
        private bool IsOrganizationalEmail(string email, string organizationDomain)
        {
            var parts = email.Split('@');
            if (parts.Length != 2) return false;
            return parts[1].Equals(organizationDomain, StringComparison.OrdinalIgnoreCase);
        }

    }

}
