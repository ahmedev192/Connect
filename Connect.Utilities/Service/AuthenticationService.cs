using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Connect.Models;
using Connect.Models.ViewModels;
using Connect.Utilities.Service.IService;
using Connect.Utilities.StaticDetails;
using Microsoft.AspNetCore.Identity;

namespace Connect.Utilities.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUsersService _usersService;

        public AuthenticationService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUsersService usersService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _usersService = usersService;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.Email?.Split('@')?.FirstOrDefault() ?? "unknown",
                Email = model.Email,
                FullName = $"{model.FirstName} {model.LastName}"
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, ApplicationRoles.User);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return result;
        }

        public async Task<SignInResult> ExternalLoginAsync()
        {
            var authResult = await _signInManager.GetExternalLoginInfoAsync();
            if (authResult == null)
                return SignInResult.Failed;

            var email = authResult.Principal.FindFirstValue(ClaimTypes.Email);
            var fullName = authResult.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
                return SignInResult.Failed;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    UserName = email,
                    FullName = fullName,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return SignInResult.Failed;

                await _userManager.AddToRoleAsync(user, ApplicationRoles.User);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return SignInResult.Success;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
