using Connect.Application.Dtos;
using Connect.Domain.Entities;

namespace Connect.Web.ViewModels
{
    public class FriendshipsViewModel
    {
        public List<FriendshipDto> Friends { get; set; } = new List<FriendshipDto>();
        public List<FriendRequestDto> SentRequests { get; set; } = new List<FriendRequestDto>();
        public List<FriendRequestDto> ReceivedRequests { get; set; } = new List<FriendRequestDto>();
    }
}
