using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Application.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Connect.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<SignInResult> LoginAsync(LoginDto model);
        Task<IdentityResult> RegisterAsync(RegisterDto model);
        Task<SignInResult> ExternalLoginAsync();
        Task LogoutAsync();
    }
}
