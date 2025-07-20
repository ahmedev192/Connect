using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Connect.Utilities.Service.IService
{
    public interface IAuthenticationService
    {
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<SignInResult> ExternalLoginAsync();
        Task LogoutAsync();
    }
}
