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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }






        [HttpPost]
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
            {
                return Redirect(returnUrl ?? "/"); // redirect to homepage or returnUrl
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel userViewModel)
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
