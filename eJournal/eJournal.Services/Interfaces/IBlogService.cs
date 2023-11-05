using eJournal.Domain.Models;

namespace eJournal.Services.Interfaces
{
    public interface IBlogService
    {
        Task<Blog> CreateBlogAsync(Blog blog);
        Task DeleteBlogAsync(int id);
        Task UpdateBlogAsync(Blog blog);
        Task<Blog> GetBlogByIdAsync(int id);
        Task<IAsyncEnumerable<Blog>> GetAllBlogsAsync(int skip = 0, int take = 0);
        IAsyncEnumerable<Blog> GetAllBlogsByUserId(int userId, int skip = 0, int take = 0);
        Task<int> SearchBlogsCountAsync(string text);
        Task<IAsyncEnumerable<Blog>> SearchBlogsAsync(string text, int skip, int blogsPerPage);
    }
}
