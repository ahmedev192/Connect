using Connect.Application.Interfaces;
using Connect.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Connect.Web.Services
{
    public class SignalRNotificationDispatcher : INotificationDispatcher
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationDispatcher(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task DispatchNotificationAsync(int userId, int unreadCount)
        {
            await _hubContext.Clients.User(userId.ToString())
                             .SendAsync("ReceiveNotification", unreadCount);
        }
    }
}
