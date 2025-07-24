using Connect.Controllers.Base;
using Connect.Domain.Entities;
using Connect.Application.Service;
using Connect.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService, UserManager<User> userManager) : base(userManager)
        {
        
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCount()
        {
            var userId = GetUserId();
            if (!userId.HasValue) RedirectToLogin();

            var count = await _notificationService.GetUnreadNotificationsCountAsync(userId.Value);
            return Json(count);
        }


        public async Task<IActionResult> GetNotifications()
        {
            var userId = GetUserId();
            if (!userId.HasValue) RedirectToLogin();

            var notifications = await _notificationService.GetNotifications(userId.Value);
            return PartialView("Notifications/_Notifications", notifications);
        }


        [HttpPost]
        public async Task<IActionResult> SetNotificationAsRead(int notificationId)
        {
            var userId = GetUserId();
            if (!userId.HasValue) RedirectToLogin();

            await _notificationService.SetNotificationAsReadAsync(notificationId);

            var notifications = await _notificationService.GetNotifications(userId.Value);
            return PartialView("Notifications/_Notifications", notifications);
        }

    }
}
