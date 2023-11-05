
using eJournal.Domain.Models;

namespace eJournal.Services.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(long blogId, long UserId);
        Task GenerateCommentNotification(long blogUserId, long commentUserId);
        Task GenerateNotificationForLike(long blogUserId, long likeUserId);
        IAsyncEnumerable<Notification> GetNotificationsByBlogId(int blogId);
        IAsyncEnumerable<Notification> GetNotificationsByUserId(int USerId);
        Task DeleteNotificationAsync(int notificationId);
        Task<Notification> GetNotificationById(int notificationId);
        Task UpdateNotificationAsync(Notification notification);
        Task GenerateNotificationForLikeInComment(long commentId, long likeUserId);
        Task GenerateNotificationForReplyInComment(long commentId, long commentUserId);
    }
}
