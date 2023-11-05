using eJournal.Domain.Models;
using eJournal.Repository;
using eJournal.Services.Interfaces;

namespace eJournal.Services.Implementions
{
    public class LikeService : ILikeService
    {
        private readonly IRepository<Like> _likeRepository;
        public LikeService(IRepository<Like> likeRepository, IRepository<Blog> blogRepository)
        {
            _likeRepository = likeRepository;
        }
        public async Task<Like> CreateLikeAsync(Like like)
        {
            try
            {
                var result = await _likeRepository.CreateAsync(like);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while creating the Like: " + ex);
            }
        }

        public async Task DeleteLikeAsync(int likeId)
        {
            try
            {
                await _likeRepository.DeleteAsync(likeId);
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while deleting the Like: " + ex);
            }
        }
        public IAsyncEnumerable<Like> GetAllLikesByBlogId(int blogId)
        {
            var result = _likeRepository.GeneralSearch(Like => Like.BlogId == blogId);
            return result;
        }
        public IAsyncEnumerable<Like> GetAllLikesByCommentId(int commentId)
        {
            var result = _likeRepository.GeneralSearch(comment => comment.CommentId == commentId);
            return result;
        }

        public async Task<Like> GetLikeById(int likeId)
        {
            try
            {
                var result = await _likeRepository.GetByIdAsync(likeId);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Like GetLikeByUserAndBlogId(int userId, int blogId)
        {
            var likes = _likeRepository.GeneralSearch(like => like.UserId == userId && like.BlogId == blogId).ToListAsync().Result.ToList();
            if (likes.Count > 0)
            {
                return likes[0];
            }
            return null;
        }

        public Like GetLikeByUserAndCommentId(int userId, int commentId)
        {
            var likes = _likeRepository.GeneralSearch(like => like.UserId == userId && like.CommentId == commentId).ToListAsync().Result.ToList();
            if (likes.Count > 0)
            {
                return likes[0];
            }
            return null;
        }
        public async Task<List<int>> GetTopFiveBlogIdsWithMostLikes()
        {
            var likes = await _likeRepository.GetAllAsync();

            var groupedLikes = likes.Where(like => like.BlogId != null)
                .GroupBy(like => like.BlogId)
                .ToListAsync();

            var blogLikesCount = new Dictionary<int, int>();

            foreach (var group in await groupedLikes)
            {
                var blogId = group.Key.Value;
                var likeCount = group.CountAsync();
                blogLikesCount.Add((int)blogId, (int)await likeCount);
            }

            var sortedBlogLikesCount = blogLikesCount.OrderByDescending(pair => pair.Value)
                                                   .ToDictionary(pair => pair.Key, pair => pair.Value);

            var topFiveBlogIds = sortedBlogLikesCount.Keys.Take(5).ToList();
            return topFiveBlogIds;
        }

    }
}
