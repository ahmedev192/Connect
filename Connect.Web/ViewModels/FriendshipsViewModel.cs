using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect.ViewModels
{
    public class FriendshipsViewModel
    {
        public List<Friendship> Friends = new List<Friendship>();
        public List<FriendRequest> SentRequests = new List<FriendRequest>();
        public List<FriendRequest> RecievedRequests = new List<FriendRequest>();



    }
}
