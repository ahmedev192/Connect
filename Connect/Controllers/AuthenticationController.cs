using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Models.ViewModels;
using Connect.Utilities.Service.IService;
using Connect.Utilities.StaticDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Connect.Controllers
{
    [Authorize]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IFileUploadService _fileUploadService;
        private readonly ApplicationDbContext _context;
        private readonly IUsersService _usersService;

        public AuthenticationController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IFileUploadService fileUploadService,
            ApplicationDbContext context,
            IUsersService usersService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileUploadService = fileUploadService;
            _context = context;
            _usersService = usersService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() => View();

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register() => View();

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return Redirect(returnUrl ?? "/");

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel userViewModel)
        {
            if (!ModelState.IsValid)
                return View(userViewModel);

            var user = new User
            {
                UserName = userViewModel.Email?.Split('@')?.FirstOrDefault() ?? "unknown",
                Email = userViewModel.Email,
                FullName = $"{userViewModel.FirstName} {userViewModel.LastName}"
            };

            var result = await _userManager.CreateAsync(user, userViewModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim("FullName", user.FullName));
                await _userManager.AddClaimAsync(user, new Claim("UserName", user.UserName));
                await _userManager.AddToRoleAsync(user, ApplicationRoles.User);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(SettingsViewModel model )
        {
            TempData["ActiveTab"] = "Profile";
            var profileModel = model.UpdateProfile;

            // Validate only UpdateProfileViewModel
            var context = new ValidationContext(profileModel);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(profileModel, context, results, true);

            if (!isValid)
            {
                TempData["UserProfileError"] = "Invalid input.";
                return RedirectToAction("Index", "Setting");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["UserProfileError"] = "User not found.";
                return RedirectToAction("Login", "Authentication");
            }

            if (user.UserName != model.UpdateProfile.UserName)
            {
                var existingUser = await _userManager.FindByNameAsync(model.UpdateProfile.UserName);
                if (existingUser != null)
                {
                    TempData["UserProfileError"] = "Username is already taken.";
                    return RedirectToAction("Index", "Setting");
                }
            }

            user.FullName = model.UpdateProfile.FullName;
            user.UserName = model.UpdateProfile.UserName;
            user.Bio = model.UpdateProfile.Bio;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["UserProfileError"] = string.Join(" ", result.Errors.Select(e => e.Description));
                return RedirectToAction("Index", "Setting");
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["UserProfileSuccess"] = "Profile updated successfully.";

            return RedirectToAction("Index", "Setting");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(SettingsViewModel model)
        {
            TempData["ActiveTab"] = "Password";

            if ( model.UpdatePassword.NewPassword != model.UpdatePassword.ConfirmNewPassword)
            {
                TempData["PasswordError"] = "Passwords do not match or input is invalid.";
                return RedirectToAction("Index", "Setting");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["PasswordError"] = "User not found.";
                return RedirectToAction("Login", "Authentication");
            }

            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, model.UpdatePassword.CurrentPassword);
            if (!isCurrentPasswordValid)
            {
                TempData["PasswordError"] = "Current password is incorrect.";
                return RedirectToAction("Index", "Setting");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.UpdatePassword.CurrentPassword, model.UpdatePassword.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["PasswordSuccess"] = "Password updated successfully.";
            }
            else
            {
                TempData["PasswordError"] = string.Join(" ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Index", "Setting");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile file)
        {
            TempData["ActiveTab"] = "Profile";

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "Setting");
            }

            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "No file was selected.";
                return RedirectToAction("Index", "Setting");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                TempData["ErrorMessage"] = "Only JPG and PNG files are allowed.";
                return RedirectToAction("Index", "Setting");
            }

            if (file.Length > 20 * 1024 * 1024)
            {
                TempData["ErrorMessage"] = "File size must be under 5MB.";
                return RedirectToAction("Index", "Setting");
            }

            if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
            {
                await _fileUploadService.DeleteImageAsync(user.ProfilePictureUrl);
            }

            var folderName = "ProfilePictures";
            var filePath = await _fileUploadService.SaveImageAsync(file, folderName);

            if (string.IsNullOrWhiteSpace(filePath))
            {
                TempData["ErrorMessage"] = "Failed to upload profile picture.";
                return RedirectToAction("Index", "Setting");
            }

            user.ProfilePictureUrl = filePath;
            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile picture updated successfully.";
            return RedirectToAction("Index", "Setting");
        }
    }


}
