using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect.Application.Dtos
{
    public class SettingsDto
    {
        public UserDto User { get; set; }
        public UpdateProfileDto UpdateProfile { get; set; }
        public UpdatePasswordDto UpdatePassword { get; set; }
    }
}
