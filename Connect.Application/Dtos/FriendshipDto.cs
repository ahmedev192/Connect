using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect.Application.Dtos
{
    public class FriendshipDto
    {
        public int FriendId { get; set; }
        public string FullName { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
