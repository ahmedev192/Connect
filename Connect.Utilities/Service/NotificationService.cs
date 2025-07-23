using System.Linq.Expressions;
using System.Threading.Tasks;
using Connect.DataAccess.Data;
using Connect.DataAccess.Hubs;
using Connect.DataAccess.Repository.IRepository;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Connect.Utilities.StaticDetails;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Connect.Utilities.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IGenericRepository<Notification> _notificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(
            IGenericRepository<Notification> notificationRepository,
            IHubContext<NotificationHub> hubContext)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
        }

        public async Task AddNewNotificationAsync(int userId, string notificationType, string userFullName, int? postId)
        {
            var newNotification = new Notification
            {
                UserId = userId,
                Message = GetPostMessage(notificationType, userFullName),
                Type = notificationType,
                IsRead = false,
                PostId = postId.HasValue ? postId.Value : null,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(newNotification);
            var notificationNumber = await GetUnreadNotificationsCountAsync(userId);

            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", notificationNumber);
        }

        public async Task<int> GetUnreadNotificationsCountAsync(int userId)
        {
            return await _notificationRepository.CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<List<Notification>> GetNotifications(int userId)
        {
            return (await _notificationRepository.GetPagedAsync(
                page: 1,
                pageSize: int.MaxValue, // Fetch all notifications
                orderBy: n => n.DateCreated,
                descending: true,
                predicate: n => n.UserId == userId,
                noTracking: true)).ToList();
        }

        public async Task SetNotificationAsReadAsync(int notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification != null)
            {
                notification.DateUpdated = DateTime.UtcNow;
                notification.IsRead = true;
                _notificationRepository.Update(notification);
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