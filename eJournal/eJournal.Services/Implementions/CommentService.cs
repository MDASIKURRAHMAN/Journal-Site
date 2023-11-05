using eJournal.Domain.Models;
using eJournal.Repository;
using eJournal.Services.Interfaces;
using System.Reflection.Metadata;

namespace eJournal.Services.Implementions
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly ILikeService _likeService;

        public CommentService(IRepository<Comment> commentRepository, ILikeService likeService)
        {
            _commentRepository = commentRepository;
            _likeService = likeService;
        }
        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            try
            {
                var result = await _commentRepository.CreateAsync(comment);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while creating the Comment: " + ex);
            }
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            try
            {
                // before deleting the actual comment we have to delete all the dependencies first.
                // delete the likes of that comment.
                var allLikes = _likeService.GetAllLikesByCommentId(commentId).ToListAsync().Result.ToList();
                foreach (var like in allLikes)
                {
                    await _likeService.DeleteLikeAsync((int)like.LikeId);
                }

                // delete the replies of that comment.
                var allReplies = GetCommentsByCommentId(commentId).ToListAsync().Result.ToList();
                foreach(var reply in allReplies)
                {
                    await DeleteCommentAsync((int)reply.CommentId);
                }

                // delete the actual comment.
                await _commentRepository.DeleteAsync(commentId);
                return;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while deleting the comment: " + ex);
            }
        }

        public async Task<Comment> GetCommentByIdAsync(int commentId)
        {
            try
            {
                var result = await _commentRepository.GetByIdAsync(commentId);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IAsyncEnumerable<Comment> GetCommentsByBlogId(int blogId)
        {
            var result = _commentRepository.GeneralSearch(comment => comment.BlogId == blogId);
            return result;
        }
        public IAsyncEnumerable<Comment> GetCommentsByCommentId(int commentId)
        {
            var result = _commentRepository.GeneralSearch(comment => comment.ParentCommentId == commentId);
            return result;
        }

        public async Task<Comment> UpdateCommentAsync(Comment comment)
        {
            try
            {
               var result = await _commentRepository.UpdateAsync(comment);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while updating the Comment: " + ex);
            }
        }
    }
}
