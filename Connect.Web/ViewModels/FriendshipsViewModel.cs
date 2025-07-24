using Connect.Domain.Entities;

namespace Connect.Web.ViewModels
{
    public class FriendshipsViewModel
    {
        public List<Friendship> Friends = new List<Friendship>();
        public List<FriendRequest> SentRequests = new List<FriendRequest>();
        public List<FriendRequest> RecievedRequests = new List<FriendRequest>();



    }
}
