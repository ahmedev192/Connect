using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Models;

namespace Connect.Utilities.Service.IService
{
    public interface INotificationService
    {
        Task AddNewNotificationAsync(int userId, string notificationType, string userFullName, int? postId);

        Task<int> GetUnreadNotificationsCountAsync(int userId);
        Task<List<Notification>> GetNotifications(int userId);




    }
}
