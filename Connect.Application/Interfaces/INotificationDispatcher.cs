using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect.Application.Interfaces
{
    public interface INotificationDispatcher
    {
        Task DispatchNotificationAsync(int userId, int unreadCount);
    }
}
