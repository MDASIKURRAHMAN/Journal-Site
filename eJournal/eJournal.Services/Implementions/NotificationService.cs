using eJournal.Domain.Models;
using eJournal.Repository;
using eJournal.Services.Interfaces;
namespace eJournal.Services.Implementions
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IUserService _userService;
        public NotificationService(IRepository<Notification> notificationRepository, IRepository<Blog> blogRepository, IRepository<User> userRepository, IRepository<Comment> commenRepository, IUserService userService)
        {
            _notificationRepository = notificationRepository;
            _blogRepository = blogRepository;
            _userRepository = userRepository;
            _commentRepository= commenRepository;
            _userService =userService;
        }

        public async Task CreateNotificationAsync(long blogId, long UserId)
        {
            var CurrentUser = await _userRepository.GetByIdAsync(UserId);
            var UserName = CurrentUser.UserName;
            var Users = await _userService.GetAllUserAsync();
            if (Users == null)
            {
                throw new Exception("user not present");
            }
            await foreach (var user in Users)
            {
                if (user.UserId == CurrentUser.UserId)
                {
                    continue;
                }

                var notification = new Notification
                {
                    UserId = user.UserId,
                    BlogId = blogId,
                    NotificationText =($"A New Post from {UserName}"),
                    IsChecked = false,
                    CreateAt = DateTime.UtcNow
                };
                try
                {
                    await _notificationRepository.CreateAsync(notification);
                }
                catch (Exception ex)
                {
                    throw new Exception("Something went wrong while saving the notification: " + ex);
                }
            }

        }
        public async Task GenerateCommentNotification(long blogId, long commentUserId)
        {
            var Blog = await _blogRepository.GetByIdAsync(blogId);
            var blogOwner = await _userRepository.GetByIdAsync(Blog.UserId);
            var commentUser = await _userRepository.GetByIdAsync(commentUserId);
            var commentUserName = commentUser.UserName;
            if (blogOwner != null && blogOwner.UserId != commentUserId)
            {
                var message = $"{commentUserName} commented on your post.";
                var notification = new Notification
                {
                    UserId = Blog.UserId,
                    BlogId = blogId,
                    NotificationText = message,
                    CreateAt = DateTime.UtcNow,
                    IsChecked = false
                };

                try
                {
                    await _notificationRepository.CreateAsync(notification);
                }
                catch (Exception ex)
                {
                    throw new Exception("Something went wrong while saving the notification: " + ex);
                }
            }
        }
        public async Task GenerateNotificationForLike(long blogId, long likeUserId)
        {
            var Blog = await _blogRepository.GetByIdAsync(blogId);
            var blogOwner = await _userRepository.GetByIdAsync(Blog.UserId);
            var likeUser = await _userRepository.GetByIdAsync(likeUserId);
            var likeUserName = likeUser.UserName;
            if (blogOwner != null && blogOwner.UserId != likeUserId)
            {
                var message = $"{likeUserName} Liked on your post.";
                var notification = new Notification
                {
                    UserId = Blog.UserId,
                    BlogId = blogId,
                    NotificationText = message,
                    CreateAt = DateTime.UtcNow,
                    IsChecked = false
                };

                try
                {
                    await _notificationRepository.CreateAsync(notification);
                }
                catch (Exception ex)
                {
                    throw new Exception("Something went wrong while saving the notification: " + ex);
                }
            }
        }
        public async Task GenerateNotificationForLikeInComment(long commentId, long likeUserId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            var commentOwner = await _userRepository.GetByIdAsync(comment.UserId);
            var likeUser = await _userRepository.GetByIdAsync(likeUserId);
            var likeUserName = likeUser.UserName;
            if (comment.BlogId == null)
            {
                var parentComment = await _commentRepository.GetByIdAsync((long)comment.ParentCommentId);
                var message = $"{likeUserName} Liked on your Comment Reply.";
                if (commentOwner != null && parentComment.UserId != likeUserId)
                {
                    var notification = new Notification
                    {

                        UserId = parentComment.UserId,
                        BlogId = (long)parentComment.BlogId,
                        NotificationText = message,
                        CreateAt = DateTime.UtcNow,
                        IsChecked = false
                    };

                    try
                    {
                        await _notificationRepository.CreateAsync(notification);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Something went wrong while saving the notification: " + ex);
                    }
                }
            }

            if (commentOwner != null && commentOwner.UserId != likeUserId && comment.BlogId != null)
            {
                var message = $"{likeUserName} Liked on your Comment.";
                var notification = new Notification
                {

                    UserId = comment.UserId,
                    BlogId = (long)comment.BlogId,
                    NotificationText = message,
                    CreateAt = DateTime.UtcNow,
                    IsChecked = false
                };

                try
                {
                    await _notificationRepository.CreateAsync(notification);
                }
                catch (Exception ex)
                {
                    throw new Exception("Something went wrong while saving the notification: " + ex);
                }
            }
        }
        public async Task GenerateNotificationForReplyInComment(long commentId, long commentUserId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            var parentComment = await _commentRepository.GetByIdAsync((long)comment.ParentCommentId);
            var commentOwner = await _userRepository.GetByIdAsync(comment.UserId);
            var commentUser = await _userRepository.GetByIdAsync(commentUserId);
            var commentUserName = commentUser.UserName;
            if (commentOwner != null && parentComment.UserId != commentUserId)
            {
                var message = $"{commentUserName} replied on your Comment.";
                var notification = new Notification
                {
                    UserId = parentComment.UserId,
                    BlogId = (long)parentComment.BlogId,
                    NotificationText = message,
                    CreateAt = DateTime.UtcNow,
                    IsChecked = false
                };

                try
                {
                    await _notificationRepository.CreateAsync(notification);
                }
                catch (Exception ex)
                {
                    throw new Exception("Something went wrong while saving the notification: " + ex);
                }
            }
        }
        public async Task UpdateNotificationAsync(Notification notification)
        {
            try
            {
                await _notificationRepository.UpdateAsync(notification);
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while updating the blog:" + ex);
            }
            return;
        }
        public async Task<Notification> GetNotificationById(int notificationId)
        {
            var result = await _notificationRepository.GetByIdAsync(notificationId);
            if (result != null)
            {
                return result;
            }
            return null;
        }
        public IAsyncEnumerable<Notification> GetNotificationsByBlogId(int blogId)
        {
            var result = _notificationRepository.GeneralSearch(notification => notification.BlogId == blogId);
            return result;
        }
        public IAsyncEnumerable<Notification> GetNotificationsByUserId(int UserId)
        {
            var result = _notificationRepository.GeneralSearch(notification => notification.UserId == UserId);
            return result;
        }
        public async Task DeleteNotificationAsync(int notificationId)
        {
            try
            {
                await _notificationRepository.DeleteAsync(notificationId);
                return;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while deleting the comment: " + ex);
            }
        }

    }
}
