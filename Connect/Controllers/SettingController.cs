using System.Security.Claims;
using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Models.ViewModels;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly UserManager<User> _userManager;

        public SettingController(IUsersService usersService, UserManager<User> userManager)
        {
            _usersService = usersService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Authentication");

            var model = new SettingsViewModel
            {
                User = user,
                UpdateProfile = new UpdateProfileViewModel
                {
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Bio = user.Bio
                },
                UpdatePassword = new UpdatePasswordViewModel()
            };

            return View(model);
        }
    }
}
