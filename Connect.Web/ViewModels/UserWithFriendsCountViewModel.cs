using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect.Web.ViewModels
{
    public class UserWithFriendsCountViewModel
    {

        public int UserId { get; set; }
        public string FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public int FriendsCount { get; set; }
        public string FriendsCountDisplay =>
            FriendsCount == 0 ? "No friends" :
            FriendsCount == 1 ? "1 friend" :
            $"{FriendsCount} friends";
    }
}
