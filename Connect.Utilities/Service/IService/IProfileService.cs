using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Connect.Models;
using Connect.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Connect.Utilities.Service.IService
{
    public interface IProfileService
    {
        Task<SettingsViewModel> GetProfileViewModelAsync(User user);
        Task<ServiceResult> UpdateProfileAsync(UpdateProfileViewModel model, ClaimsPrincipal user);
        Task<ServiceResult> UpdatePasswordAsync(UpdatePasswordViewModel model, ClaimsPrincipal user);
        Task<ServiceResult> UpdateProfilePictureAsync(IFormFile file, ClaimsPrincipal user);
    }

    public class ServiceResult
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
