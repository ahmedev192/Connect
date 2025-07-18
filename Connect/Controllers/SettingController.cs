using System.Security.Claims;
using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IFileUploadService _fileUploadService;
        private readonly ApplicationDbContext _context;
        public SettingController(IUsersService usersService, IFileUploadService fileUploadService, ApplicationDbContext context)
        {
            _usersService = usersService;
            _fileUploadService = fileUploadService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUserId = 1;
            var user = await _usersService.GetUser(loggedInUserId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile file)
        {
            var userId = 1; // Replace with actual user ID retrieval logic

            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "No file was selected.";
                return View("Index");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                TempData["ErrorMessage"] = "Only JPG and PNG files are allowed.";
                return View("Index");
            }

            if (file.Length > 20 * 1024 * 1024)
            {
                TempData["ErrorMessage"] = "File size must be under 5MB.";
                return View("Index");
            }

            var user = await _usersService.GetUser(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return View("Index");
            }

            // Delete old profile picture if it exists and isn't a default image
            if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
            {
                await _fileUploadService.DeleteImageAsync(user.ProfilePictureUrl);
            }

            // Save new profile picture
            var folderName = "ProfilePictures";
            var filePath = await _fileUploadService.SaveImageAsync(file, folderName);

            if (string.IsNullOrWhiteSpace(filePath))
            {
                TempData["ErrorMessage"] = "Failed to upload profile picture.";
                return View("Index");
            }

            user.ProfilePictureUrl = filePath;
            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile picture updated successfully.";
            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User user)
        {
            var userId = 1;
            var existingUser = await _usersService.GetUser(userId);
            if (existingUser == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return View("Index");
            }
            existingUser.FullName = user.FullName;
            _context.Update(existingUser);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Profile updated successfully.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(User user)
        {
            var userId = 1;

            return RedirectToAction("Index");
        }
    }
}
