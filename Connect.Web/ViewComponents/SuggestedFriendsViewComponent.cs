using Connect.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Connect.Domain.Entities;
using Connect.Web.ViewModels;

namespace Connect.ViewComponents
{
    public class SuggestedFriendsViewComponent : ViewComponent
    {
        private readonly IFriendService _friendService;
        private readonly UserManager<User> _userManager;
        public SuggestedFriendsViewComponent(IFriendService friendService, UserManager<User> userManager)
        {
            _friendService = friendService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id ?? 0;

            var suggestedFriends = await _friendService.GetSuggestedFriendsAsync(userId);
            var suggestedFriendsVM = suggestedFriends.Select(n => new UserWithFriendsCountViewModel()
            {
                UserId = n.User.Id,
                FullName = n.User.FullName,
                ProfilePictureUrl = n.User.ProfilePictureUrl,
                FriendsCount = n.FriendsCount
            }).ToList();

            return View(suggestedFriendsVM);
        }
    }
}
