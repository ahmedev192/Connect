using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Connect.Application.Dtos;
using Connect.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Connect.Application.Interfaces
{
    public interface IProfileService
    {
        Task<SettingsDto> GetProfileViewModelAsync(UserDto user);
        Task<ServiceResult> UpdateProfileAsync(UpdateProfileDto model, ClaimsPrincipal user);
        Task<ServiceResult> UpdatePasswordAsync(UpdatePasswordDto model, ClaimsPrincipal user);
        Task<ServiceResult> UpdateProfilePictureAsync(IFormFile file, ClaimsPrincipal user);
    }

    public class ServiceResult
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
