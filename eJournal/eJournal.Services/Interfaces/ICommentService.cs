using eJournal.Domain.Models;

namespace eJournal.Services.Interfaces
{
    public interface ICommentService
    {
        Task<Comment> CreateCommentAsync(Comment comment);
        IAsyncEnumerable<Comment> GetCommentsByBlogId(int blogId);
        IAsyncEnumerable<Comment> GetCommentsByCommentId(int commentId);
        Task<Comment> GetCommentByIdAsync(int commentId);
        Task<Comment> UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(int commentId);
    }
}
