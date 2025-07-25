using Connect.Application.Dtos;
using Connect.Application.Interfaces;
using Connect.Application.StaticDetails;
using Connect.Controllers.Base;
using Connect.Domain.Entities;
using Connect.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    public class FriendController : BaseController
    {
        private readonly IFriendService _friendService;
        private readonly INotificationService _notificationsService;

        public FriendController(IFriendService friendService, UserManager<User> userManager, INotificationService notificationService) : base(userManager)
        {
            _friendService = friendService;
            _notificationsService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (!userId.HasValue) return RedirectToLogin();

            var friends = await _friendService.GetFriendsAsync(userId.Value);
            var sentRequests = await _friendService.GetSentFriendRequestAsync(userId.Value);
            var receivedRequests = await _friendService.GetReceivedFriendRequestAsync(userId.Value);

            var friendsData = new FriendshipsViewModel
            {
                Friends = friends.Select(f => new FriendshipDto
                {
                    Id = f.Id,
                    FriendId = f.User1Id == userId.Value ? f.User2Id : f.User1Id,
                    FullName = f.User1Id == userId.Value ? f.User2.FullName : f.User1.FullName,
                    ProfilePictureUrl = f.User1Id == userId.Value
                        ? f.User2.ProfilePictureUrl ?? "/images/avatars/user.png"
                        : f.User1.ProfilePictureUrl ?? "/images/avatars/user.png"
                }).ToList(),
                SentRequests = sentRequests.Select(r => new FriendRequestDto
                {
                    RequestId = r.Id,
                    SenderId = r.SenderId,
                    SenderFullName = r.Sender.FullName,
                    SenderProfilePictureUrl = r.Sender.ProfilePictureUrl ?? "/images/avatars/user.png",
                    ReceiverId = r.ReceiverId,
                    ReceiverFullName = r.Receiver.FullName,
                    ReceiverProfilePictureUrl = r.Receiver.ProfilePictureUrl ?? "/images/avatars/user.png",
                    SentAt = r.DateCreated
                }).ToList(),
                ReceivedRequests = receivedRequests.Select(r => new FriendRequestDto
                {
                    RequestId = r.Id,
                    SenderId = r.SenderId,
                    SenderFullName = r.Sender.FullName,
                    SenderProfilePictureUrl = r.Sender.ProfilePictureUrl ?? "/images/avatars/user.png",
                    ReceiverId = r.ReceiverId,
                    ReceiverFullName = r.Receiver.FullName,
                    ReceiverProfilePictureUrl = r.Receiver.ProfilePictureUrl ?? "/images/avatars/user.png",
                    SentAt = r.DateCreated
                }).ToList()
            };

            return View(friendsData);
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(int receiverId)
        {
            var userId = GetUserId();
            var userName = GetUserFullName();
            if (!userId.HasValue) return RedirectToLogin();

            await _friendService.SendRequestAsync(userId.Value, receiverId);
            await _notificationsService.AddNewNotificationAsync(receiverId, NotificationType.FriendRequest, userName, null);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFriendRequest(int requestId, FriendRequestStatus status)
        {
            var userId = GetUserId();
            var userName = GetUserFullName();
            if (!userId.HasValue) return RedirectToLogin();

            var request = await _friendService.UpdateRequestAsync(requestId, status);

            if (status == FriendRequestStatus.Accepted)
                await _notificationsService.AddNewNotificationAsync(request.SenderId, NotificationType.FriendRequestApproved, userName, null);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int friendshipId)
        {
            await _friendService.RemoveFriendAsync(friendshipId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CancelFriendRequest(int requestId)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return RedirectToLogin();

            await _friendService.CancelFriendRequestAsync(requestId, userId.Value);
            return RedirectToAction("Index");
        }
    }
}