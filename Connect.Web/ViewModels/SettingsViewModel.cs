using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect.ViewModels
{
    public class SettingsViewModel
    {
        public User User { get; set; } = null!;
        public UpdateProfileViewModel UpdateProfile { get; set; } = new();
        public UpdatePasswordViewModel UpdatePassword { get; set; } = new();
    }
}
