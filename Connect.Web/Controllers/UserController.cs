using Connect.Domain;

using Connect.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    public class UserController : Controller
    {
        private readonly IUsersService _userService;
        private readonly UserManager<User> _userManager;


        public UserController(IUsersService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> UserDetails(int userId)
        {
            var userPosts = await _userService.GetPostsByUserId(userId);
            var user = await _userManager.GetUserAsync(User);
            UserProfileViewModel userProfileVM = new UserProfileViewModel
            {
                User = user,
                Posts = userPosts
            };
            return View(userProfileVM);

        }
    }
}
