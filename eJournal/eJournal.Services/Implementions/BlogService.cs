using eJournal.Domain.Models;
using eJournal.Repository;
using eJournal.Services.Interfaces;

namespace eJournal.Services.Implementions
{
    public class BlogService : IBlogService
    {
        private readonly IRepository<Blog> _blogRepository;
        private readonly ILikeService _likeService;
        private readonly ICommentService _commentService;

        public BlogService(
            IRepository<Blog> blogRepository,
            ILikeService likeService,
            ICommentService commentService
            )
        {
            _blogRepository = blogRepository;
            _likeService = likeService;
            _commentService = commentService;
        }
        public async Task<Blog> CreateBlogAsync(Blog blog)
        {
            try
            {
                var result = await _blogRepository.CreateAsync(blog);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while creating the blog: " + ex);
            }
        }

        public async Task DeleteBlogAsync(int id)
        {
            try
            {
                //delete the likes first.
                var allLikes = _likeService.GetAllLikesByBlogId(id).ToListAsync().Result.ToList();
                foreach (var like in allLikes)
                {
                    await _likeService.DeleteLikeAsync((int)like.LikeId);
                }

                //delete the comments. 
                var allComments = _commentService.GetCommentsByBlogId(id).ToListAsync().Result.ToList();
                foreach (var comment in allComments)
                {
                    await _commentService.DeleteCommentAsync((int)comment.CommentId);
                }

                //delete the actual post.
                await _blogRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while deleting the blog:" + ex);
            }
            return;
        }

        public async Task<IAsyncEnumerable<Blog>> GetAllBlogsAsync(int skip = 0, int take = 0)
        {
            var result = await _blogRepository.GetAllAsync((blog => blog.CreatedAt), skip, take);
            return result;
        }

        public IAsyncEnumerable<Blog> GetAllBlogsByUserId(int userId, int skip = 0, int take = 0)
        {
            if (skip == 0 && take == 0)
            {
                var res = _blogRepository.GeneralSearch(blog => blog.UserId == userId);
                return res;
            }
            var result = _blogRepository.GeneralSearch(blog => blog.UserId == userId, blog => blog.CreatedAt, skip, take);
            return result;
        }

        public async Task<Blog> GetBlogByIdAsync(int id)
        {
            var result = await _blogRepository.GetByIdAsync(id);
            if (result != null)
            {
                return result;
            }
            return null;
        }

        public async Task UpdateBlogAsync(Blog blog)
        {
            try
            {
                await _blogRepository.UpdateAsync(blog);
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while updating the blog:" + ex);
            }
            return;
        }

        public async Task<IAsyncEnumerable<Blog>> SearchBlogsAsync(string searchText, int skip = 0, int take = 0)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return await GetAllBlogsAsync(skip, take);
            }
            var result = _blogRepository.GeneralSearch(blog => blog.BlogText.Contains(searchText)  || blog.BlogTitle.Contains(searchText), blog => blog.CreatedAt, skip, take);
            return result;
        }

        public async Task<int> SearchBlogsCountAsync(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return await GetAllBlogsAsync().Result.CountAsync();
            }
            return await _blogRepository.GeneralSearch(blog => blog.BlogText.Contains(searchText) || blog.BlogTitle.Contains(searchText)).CountAsync();
        }
    }
}