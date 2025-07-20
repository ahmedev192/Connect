using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Models.ViewModels;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Connect.Utilities.Service
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IFileUploadService _fileUploadService;
        private readonly ApplicationDbContext _context;

        public ProfileService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IFileUploadService fileUploadService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileUploadService = fileUploadService;
            _context = context;
        }

        public async Task<SettingsViewModel> GetProfileViewModelAsync(User user)
        {
            return new SettingsViewModel
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
        }

        public async Task<ServiceResult> UpdateProfileAsync(UpdateProfileViewModel model, ClaimsPrincipal claimsPrincipal)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, context, results, true))
                return new ServiceResult { Succeeded = false, ErrorMessage = "Invalid input." };

            var user = await _userManager.GetUserAsync(claimsPrincipal);
            if (user == null)
                return new ServiceResult { Succeeded = false, ErrorMessage = "User not found." };

            if (user.UserName != model.UserName)
            {
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null)
                    return new ServiceResult { Succeeded = false, ErrorMessage = "Username is already taken." };
            }

            user.FullName = model.FullName;
            user.UserName = model.UserName;
            user.Bio = model.Bio;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new ServiceResult { Succeeded = false, ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description)) };

            await _signInManager.RefreshSignInAsync(user);
            return new ServiceResult { Succeeded = true };
        }

        public async Task<ServiceResult> UpdatePasswordAsync(UpdatePasswordViewModel model, ClaimsPrincipal claimsPrincipal)
        {
            if (model.NewPassword != model.ConfirmNewPassword)
                return new ServiceResult { Succeeded = false, ErrorMessage = "Passwords do not match or input is invalid." };

            var user = await _userManager.GetUserAsync(claimsPrincipal);
            if (user == null)
                return new ServiceResult { Succeeded = false, ErrorMessage = "User not found." };

            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!isCurrentPasswordValid)
                return new ServiceResult { Succeeded = false, ErrorMessage = "Current password is incorrect." };

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
                return new ServiceResult { Succeeded = false, ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description)) };

            await _signInManager.RefreshSignInAsync(user);
            return new ServiceResult { Succeeded = true };
        }

        public async Task<ServiceResult> UpdateProfilePictureAsync(IFormFile file, ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);
            if (user == null)
                return new ServiceResult { Succeeded = false, ErrorMessage = "User not found." };

            if (file == null || file.Length == 0)
                return new ServiceResult { Succeeded = false, ErrorMessage = "No file was selected." };

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return new ServiceResult { Succeeded = false, ErrorMessage = "Only JPG and PNG files are allowed." };

            if (file.Length > 20 * 1024 * 1024)
                return new ServiceResult { Succeeded = false, ErrorMessage = "File size must be under 5MB." };

            if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
                await _fileUploadService.DeleteImageAsync(user.ProfilePictureUrl);

            var folderName = "ProfilePictures";
            var filePath = await _fileUploadService.SaveImageAsync(file, folderName);

            if (string.IsNullOrWhiteSpace(filePath))
                return new ServiceResult { Succeeded = false, ErrorMessage = "Failed to upload profile picture." };

            user.ProfilePictureUrl = filePath;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return new ServiceResult { Succeeded = true };
        }
    }
}
