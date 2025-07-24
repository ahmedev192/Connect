using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Domain.Entities;

namespace Connect.Web.ViewModels
{
    public class SettingsViewModel
    {
        public User User { get; set; } = null!;
        public UpdateProfileViewModel UpdateProfile { get; set; } = new();
        public UpdatePasswordViewModel UpdatePassword { get; set; } = new();
    }
}
