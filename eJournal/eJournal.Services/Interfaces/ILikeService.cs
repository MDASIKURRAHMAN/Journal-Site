using eJournal.Domain.Models;

namespace eJournal.Services.Interfaces
{
    public interface ILikeService
    {
        Task<Like> CreateLikeAsync(Like like);
        Task DeleteLikeAsync(int likeId);
        Task<Like> GetLikeById(int likeId);
        Like GetLikeByUserAndBlogId(int userId, int blogId);
        Like GetLikeByUserAndCommentId(int userId, int commentId);
        IAsyncEnumerable<Like> GetAllLikesByBlogId(int blogId);
        IAsyncEnumerable<Like> GetAllLikesByCommentId(int commentId);
        Task<List<int>> GetTopFiveBlogIdsWithMostLikes();
    }
}
