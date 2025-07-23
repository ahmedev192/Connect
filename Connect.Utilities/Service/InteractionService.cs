using System.Linq.Expressions;
using System.Threading.Tasks;
using Connect.DataAccess.Data;
using Connect.DataAccess.Repository.IRepository;
using Connect.Models;
using Connect.Models.DTOs;
using Connect.Utilities.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace Connect.Utilities.Service
{
    public class InteractionService : IInteractionService
    {
        private readonly IGenericRepository<Post> _postRepository;
        private readonly IGenericRepository<Like> _likeRepository;
        private readonly IGenericRepository<Comment> _commentRepository;
        private readonly IGenericRepository<Favorite> _favoriteRepository;
        private readonly IGenericRepository<Report> _reportRepository;
        private readonly INotificationService _notificationService;

        public InteractionService(
            IGenericRepository<Post> postRepository,
            IGenericRepository<Like> likeRepository,
            IGenericRepository<Comment> commentRepository,
            IGenericRepository<Favorite> favoriteRepository,
            IGenericRepository<Report> reportRepository,
            INotificationService notificationService)
        {
            _postRepository = postRepository;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
            _favoriteRepository = favoriteRepository;
            _reportRepository = reportRepository;
            _notificationService = notificationService;
        }

        public async Task<NotificationDTO> TogglePostLikeAsync(int postId, int userId)
        {
            var response = new NotificationDTO
            {
                Success = true,
                SendNotification = false
            };

            var post = await _postRepository.FirstOrDefaultAsync(
                p => p.Id == postId,
                noTracking: true,
                p => p.Likes);

            if (post == null)
                throw new Exception("Post not found");

            var existingLike = await _likeRepository.FirstOrDefaultAsync(
                pl => pl.PostId == postId && pl.UserId == userId);

            if (existingLike != null)
            {
                _likeRepository.Remove(existingLike);
            }
            else
            {
                await _likeRepository.AddAsync(new Like { PostId = postId, UserId = userId });
                response.SendNotification = true;
            }

            return response;
        }

        public async Task AddCommentAsync(Comment comment, int userId)
        {
            comment.UserId = userId;
            comment.DateCreated = DateTime.UtcNow;
            comment.DateUpdated = DateTime.UtcNow;
            await _commentRepository.AddAsync(comment);
        }

        public async Task<int?> DeleteCommentAsync(int commentId, int userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null || comment.UserId != userId)
                return null;

            _commentRepository.Remove(comment);
            return comment.PostId;
        }

        public async Task<NotificationDTO> TogglePostFavoriteAsync(int postId, int userId)
        {
            var response = new NotificationDTO
            {
                Success = true,
                SendNotification = false
            };

            var favorite = await _favoriteRepository.FirstOrDefaultAsync(
                f => f.PostId == postId && f.UserId == userId);

            if (favorite != null)
            {
                _favoriteRepository.Remove(favorite);
            }
            else
            {
                await _favoriteRepository.AddAsync(new Favorite { PostId = postId, UserId = userId });
                response.SendNotification = true;
            }

            return response;
        }

        public async Task AddPostReportAsync(int postId, int userId)
        {
            var existingReport = await _reportRepository.FirstOrDefaultAsync(
                r => r.PostId == postId && r.UserId == userId);

            if (existingReport != null)
                throw new Exception("You have already reported this post.");

            var report = new Report
            {
                PostId = postId,
                UserId = userId,
                DateCreated = DateTime.UtcNow
            };
            await _reportRepository.AddAsync(report);

            var post = await _postRepository.GetByIdAsync(postId);
            if (post != null)
            {
                post.NrOfReports += 1;
                _postRepository.Update(post);
            }
        }
    }
}