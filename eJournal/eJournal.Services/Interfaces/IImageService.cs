using eJournal.Domain.Models;

namespace eJournal.Services.Interfaces
{
    public interface IImageService
    {
        Task<Image> CreateImageAsync(Image image);
        Task DeleteImageAsync(long id);
        Task UpdateImageAsync(Image image);
        Task<Image> GetImageByIdAsync(long id);
        Task<List<Image>> GetImageByBlogIdAsync(long id);
    }
}
