using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Connect.Models;
using Connect.Models.ViewModels;
using Connect.Utilities.Service;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly UserManager<User> _userManager;
        private readonly IFileUploadService _fileUploadService;

        public ProfileController(IProfileService profileService, UserManager<User> userManager, IFileUploadService fileUploadService)
        {
            _profileService = profileService;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

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

            model.User.ProfilePictureUrl = _fileUploadService.ResolveImageOrDefault(model.User.ProfilePictureUrl, "/images/avatars/user.png");



            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(SettingsViewModel model)
        {
            TempData["ActiveTab"] = "Profile";
            var result = await _profileService.UpdateProfileAsync(model.UpdateProfile, User);
            if (result.Succeeded)
            {
                TempData["UserProfileSuccess"] = "Profile updated successfully.";
                return RedirectToAction("Index");
            }

            TempData["UserProfileError"] = result.ErrorMessage;
            return RedirectToAction("Index" );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(SettingsViewModel model)
        {
            TempData["ActiveTab"] = "Password";
            var result = await _profileService.UpdatePasswordAsync(model.UpdatePassword, User);
            if (result.Succeeded)
            {
                TempData["PasswordSuccess"] = "Password updated successfully.";
                return RedirectToAction("Index");
            }

            TempData["PasswordError"] = result.ErrorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile file)
        {
            TempData["ActiveTab"] = "Profile";
            var result = await _profileService.UpdateProfilePictureAsync(file, User);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile picture updated successfully.";
                return RedirectToAction("Index", "Profile");
            }

            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction("Index");
        }
    }
}