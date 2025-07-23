using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Connect.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<SignInResult> ExternalLoginAsync();
        Task LogoutAsync();
    }
}
