using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Connect.Application.Dtos;
using Connect.Application.Interfaces;
using Connect.Domain.Entities;
using Connect.Web.ViewModels;
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
        private readonly IMapper _mapper;

        public ProfileController(IProfileService profileService, UserManager<User> userManager, IFileUploadService fileUploadService, IMapper mapper)
        {
            _profileService = profileService;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
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
            if (!ModelState.IsValid)
            {
                TempData["UserProfileError"] = "Invalid profile data.";
                return RedirectToAction("Index");
            }

            // Map UpdateProfileViewModel to UpdateProfileDto
            var updateProfileDto = _mapper.Map<UpdateProfileDto>(model.UpdateProfile);

            // Call service with DTO
            var result = await _profileService.UpdateProfileAsync(updateProfileDto, User);
            if (result.Succeeded)
            {
                TempData["UserProfileSuccess"] = "Profile updated successfully.";
                return RedirectToAction("Index");
            }

            TempData["UserProfileError"] = result.ErrorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(SettingsViewModel model)
        {
            TempData["ActiveTab"] = "Password";
            // Create a new validation context for UpdatePassword
            var validationContext = new ValidationContext(model.UpdatePassword);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(model.UpdatePassword, validationContext, validationResults, true);

            if (!isValid)
            {
                TempData["PasswordError"] = "Invalid password data.";
                return RedirectToAction("Index");
            }

            // Map UpdatePasswordViewModel to UpdatePasswordDto
            var updatePasswordDto = _mapper.Map<UpdatePasswordDto>(model.UpdatePassword);

            // Call service with DTO
            var result = await _profileService.UpdatePasswordAsync(updatePasswordDto, User);
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