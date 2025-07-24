
using Connect.Infrastructure.Repository.IRepository;
using Connect.Application.Interfaces;
using Connect.Domain.Dtos;
using Connect.Domain.Entities;

namespace Connect.Application.Service
{
    public class InteractionService : IInteractionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public InteractionService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<NotificationDto> TogglePostLikeAsync(int postId, int userId)
        {
            var response = new NotificationDto
            {
                Success = true,
                SendNotification = false
            };

            var post = await _unitOfWork.PostRepository.FirstOrDefaultAsync(
                p => p.Id == postId,
                noTracking: true,
                p => p.Likes);

            if (post == null)
                throw new Exception("Post not found");

            var existingLike = await _unitOfWork.LikeRepository.FirstOrDefaultAsync(
                pl => pl.PostId == postId && pl.UserId == userId);

            if (existingLike != null)
            {
                _unitOfWork.LikeRepository.Remove(existingLike);
            }
            else
            {
                await _unitOfWork.LikeRepository.AddAsync(new Like { PostId = postId, UserId = userId });
                response.SendNotification = true;
            }

            await _unitOfWork.SaveChangesAsync();
            return response;
        }

        public async Task AddCommentAsync(Comment comment, int userId)
        {
            comment.UserId = userId;
            comment.DateCreated = DateTime.UtcNow;
            comment.DateUpdated = DateTime.UtcNow;
            await _unitOfWork.CommentRepository.AddAsync(comment);
        }

        public async Task<int?> DeleteCommentAsync(int commentId, int userId)
        {
            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(commentId);
            if (comment == null || comment.UserId != userId)
                return null;

            _unitOfWork.CommentRepository.Remove(comment);
            await _unitOfWork.SaveChangesAsync();
            return comment.PostId;
        }

        public async Task<NotificationDto> TogglePostFavoriteAsync(int postId, int userId)
        {
            var response = new NotificationDto
            {
                Success = true,
                SendNotification = false
            };

            var favorite = await _unitOfWork.FavoriteRepository.FirstOrDefaultAsync(
                f => f.PostId == postId && f.UserId == userId);

            if (favorite != null)
            {
                _unitOfWork.FavoriteRepository.Remove(favorite);
            }
            else
            {
                await _unitOfWork.FavoriteRepository.AddAsync(new Favorite { PostId = postId, UserId = userId });
                response.SendNotification = true;
            }

            await _unitOfWork.SaveChangesAsync();
            return response;
        }

        public async Task AddPostReportAsync(int postId, int userId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var existingReport = await _unitOfWork.ReportRepository.FirstOrDefaultAsync(
                    r => r.PostId == postId && r.UserId == userId);

                if (existingReport != null)
                    throw new Exception("You have already reported this post.");

                var report = new Report
                {
                    PostId = postId,
                    UserId = userId,
                    DateCreated = DateTime.UtcNow
                };
                await _unitOfWork.ReportRepository.AddAsync(report);

                var post = await _unitOfWork.PostRepository.GetByIdAsync(postId);
                if (post != null)
                {
                    post.NrOfReports += 1;
                    _unitOfWork.PostRepository.Update(post);
                }
            });
        }
    }
}