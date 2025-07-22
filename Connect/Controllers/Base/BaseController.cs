using System.Security.Claims;
using Connect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers.Base
{

    public abstract class BaseController : Controller
    {
        private readonly UserManager<User> _userManager;

        protected BaseController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected int? GetUserId()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return null;

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(loggedInUserId, out var id) ? id : null;
        }

        protected string? GetUserFullName()
        {
            var loggedInUserFullName = User.FindFirstValue(ClaimTypes.Name);
            return loggedInUserFullName;
        }

        protected Task<User?> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(User);
        }

        protected IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Account");
        }
    }
}

