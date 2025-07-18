using Connect.Models;
using Connect.Models.ViewModels;
using Connect.Utilities.StaticDetails;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userViewModel);
            }

            var user = new User
            {
                UserName = userViewModel.Email?.Split('@')?.FirstOrDefault() ?? "unknown",
                Email = userViewModel.Email,
                FullName = $"{userViewModel.FirstName} {userViewModel.LastName}"
            };

            var result = await _userManager.CreateAsync(user, userViewModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, ApplicationRoles.User);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Add registration errors to ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(userViewModel);
        }

    }
}
