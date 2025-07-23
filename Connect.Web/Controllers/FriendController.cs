using Connect.Controllers.Base;
using Connect.Domain;

using Connect.Application.Service;
using Connect.Application.Interfaces;
using Connect.Application.StaticDetails;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    public class FriendController : BaseController
    {

        private readonly IFriendService _friendService;
        private readonly INotificationService _notificationsService;

        public FriendController(IFriendService friendService, UserManager<User> userManager,INotificationService notificationService ) : base(userManager)
        {

            _friendService = friendService;
            _notificationsService = notificationService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (!userId.HasValue) RedirectToLogin();

            var friendsData = new FriendshipsViewModel()
            {
                Friends = await _friendService.GetFriendsAsync(userId.Value),
                SentRequests = await _friendService.GetSentFriendRequestAsync(userId.Value),
                RecievedRequests = await _friendService.GetReceivedFriendRequestAsync(userId.Value)
            };

            return View(friendsData);

        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(int receiverId)
        {
            var userId = GetUserId();
            var userName = GetUserFullName();
            if (!userId.HasValue) RedirectToLogin();

            await _friendService.SendRequestAsync(userId.Value, receiverId);
            await _notificationsService.AddNewNotificationAsync(receiverId, NotificationType.FriendRequest, userName, null);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFriendRequest(int requestId, string status)
        {
            var userId = GetUserId();
            var userName = GetUserFullName();
            if (!userId.HasValue) RedirectToLogin();

            var request = await _friendService.UpdateRequestAsync(requestId, status);

            if (status == FriendshipStatus.Accepted)
                await _notificationsService.AddNewNotificationAsync(request.SenderId, NotificationType.FriendRequestApproved, userName, null);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int friendshipId)
        {
            await _friendService.RemoveFriendAsync(friendshipId);
            return RedirectToAction("Index");
        }

    }
}
