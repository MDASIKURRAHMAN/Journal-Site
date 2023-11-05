using eJournal.Domain.Models;
using eJournal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eJournal.Web.Controllers
{
    public class LikeController : Controller
    {
        private readonly ILikeService _likeService;
        private readonly INotificationService _notificationService;

        public LikeController(ILikeService likeService, INotificationService notificationService)
        {
            _likeService = likeService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> GetAllLikesByBlogId(int blogId)
        {
            var totalLikesCount = await _likeService.GetAllLikesByBlogId(blogId).CountAsync();
            return Ok(totalLikesCount);// we may need to send a ViewModel for further functionality.
        }

        [HttpPost]
        public async Task<IActionResult> Create(int blogId, int commentId)
        {
            Like like = new()
            {
                UserId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value)
            };

            if (blogId > 0)
            {
                like.BlogId= blogId;
            }
            else if (commentId > 0)
            {
                like.CommentId= commentId;
            }
            else
            {
                return BadRequest("Neither blogId nor commentId has been provided");
            }
            try
            {
                Like createdLike = await _likeService.CreateLikeAsync(like);
                if (createdLike.BlogId != null)
                {
                    await _notificationService.GenerateNotificationForLike(blogId, like.UserId);
                }
                else
                {
                    await _notificationService.GenerateNotificationForLikeInComment((long)like.CommentId, like.UserId);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Could not create the like, may be bad input or server error: " + ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int blogId, int commentId)
        {
            int userId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);
            Like like = new();
            if (blogId > 0)
            {
                like = _likeService.GetLikeByUserAndBlogId(userId, blogId);
            }
            else if (commentId > 0)
            {
                like = _likeService.GetLikeByUserAndCommentId(userId, commentId);
            }
            else
            {
                return BadRequest("Neither blogId nor commentId has been provided");
            }
            try
            {
                await _likeService.DeleteLikeAsync((int)like.LikeId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Could not delete the like with id = " + like.LikeId + " " + ex);
            }
        }
    }
}
