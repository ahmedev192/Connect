using Connect.Controllers.Base;
using Connect.Models;
using Connect.Models.ViewModels;
using Connect.Utilities.Service;
using Connect.Utilities.Service.IService;
using Connect.Utilities.StaticDetails;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    public class FriendController : BaseController
    {

        private readonly IFriendService _friendService;

        public FriendController(IFriendService friendService, UserManager<User> userManager) : base(userManager)
        {

            _friendService = friendService;
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
            if (!userId.HasValue) RedirectToLogin();

            await _friendService.SendRequestAsync(userId.Value, receiverId);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFriendRequest(int requestId, string status)
        {
            await _friendService.UpdateRequestAsync(requestId, status);
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
