using System.Linq.Expressions;
using System.Threading.Tasks;
using Connect.Infrastructure.Repository;
using Connect.Infrastructure.Repository.IRepository;
using Connect.Domain.Entities;
using Connect.Application.Interfaces;
using Connect.Application.StaticDetails;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Connect.Domain.Entities;

namespace Connect.Application.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationDispatcher _notificationDispatcher;

        public NotificationService(IUnitOfWork unitOfWork, INotificationDispatcher notificationDispatcher)
        {
            _unitOfWork = unitOfWork;
            _notificationDispatcher = notificationDispatcher;
        }
        public async Task AddNewNotificationAsync(int userId, string notificationType, string userFullName, int? postId)
        {
            var newNotification = new Notification
            {
                UserId = userId,
                Message = GetPostMessage(notificationType, userFullName),
                Type = notificationType,
                IsRead = false,
                PostId = postId ?? null,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            await _unitOfWork.NotificationRepository.AddAsync(newNotification);
            var notificationNumber = await GetUnreadNotificationsCountAsync(userId);

            await _notificationDispatcher.DispatchNotificationAsync(userId, notificationNumber);
        }


        public async Task<int> GetUnreadNotificationsCountAsync(int userId)
        {
            return await _unitOfWork.NotificationRepository.CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<List<Notification>> GetNotifications(int userId)
        {
            return (await _unitOfWork.NotificationRepository.GetPagedAsync(
                page: 1,
                pageSize: int.MaxValue, // Fetch all notifications
                orderBy: n => n.DateCreated,
                descending: true,
                predicate: n => n.UserId == userId,
                noTracking: true)).ToList();
        }

        public async Task SetNotificationAsReadAsync(int notificationId)
        {
            var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(notificationId);
            if (notification != null)
            {
                notification.DateUpdated = DateTime.UtcNow;
                notification.IsRead = true;
                _unitOfWork.NotificationRepository.Update(notification);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private string GetPostMessage(string notificationType, string userFullName)
        {
            var message = "";

            switch (notificationType)
            {
                case NotificationType.Like:
                    message = $"{userFullName} liked your post";
                    break;

                case NotificationType.Favorite:
                    message = $"{userFullName} favorited your post";
                    break;

                case NotificationType.Comment:
                    message = $"{userFullName} added a comment to your post";
                    break;

                case NotificationType.FriendRequest:
                    message = $"{userFullName} sent you a friend request";
                    break;

                case NotificationType.FriendRequestApproved:
                    message = $"{userFullName} approved your friendship request";
                    break;

                default:
                    message = "";
                    break;
            }

            return message;
        }
    }
}