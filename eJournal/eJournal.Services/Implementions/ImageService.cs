using eJournal.Domain.Models;
using eJournal.Repository;
using eJournal.Services.Interfaces;

namespace eJournal.Services.Implementions
{
    public class ImageService : IImageService
    {
        private readonly IRepository<Image> _imageRepository;

        public ImageService(IRepository<Image> imageRepository)
        {
            _imageRepository = imageRepository;
        }
        public async Task<Image> CreateImageAsync(Image image)
        {
            try
            {
                var result = await _imageRepository.CreateAsync(image);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while creating the image: " + ex);
            }
        }
        public async Task DeleteImageAsync(long id)
        {
            var result = GetImageByIdAsync(id);
            if (result != null)
            {
                await _imageRepository.DeleteAsync(id);
            }
        }

        public async Task<Image> GetImageByIdAsync(long id)
        {
            var result = await _imageRepository.GetByIdAsync(id);
            if (result != null)
            {
                return result;
            }
            return null;
        }
        public async Task<List<Image>> GetImageByBlogIdAsync(long id)
        {
            var result = await _imageRepository.GeneralSearch(x => x.BlogId == id).ToListAsync();
            if (result != null)
            {
                return result;
            }
            return null;
        }

        public async Task UpdateImageAsync(Image image)
        {
            try
            {
                await _imageRepository.UpdateAsync(image);
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while updating the image:" + ex);
            }
            return;
        }
    }
}
