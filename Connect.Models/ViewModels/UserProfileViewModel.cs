using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public List<Post> Posts { get; set; }
    }
}
