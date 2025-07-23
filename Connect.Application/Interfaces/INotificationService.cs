using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Domain;

namespace Connect.Application.Interfaces
{
    public interface INotificationService
    {
        Task AddNewNotificationAsync(int userId, string notificationType, string userFullName, int? postId);

        Task<int> GetUnreadNotificationsCountAsync(int userId);
        Task<List<Notification>> GetNotifications(int userId);
        Task SetNotificationAsReadAsync(int notificationId);





    }
}
