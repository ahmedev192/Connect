using Connect.Application.Dtos;
using Connect.Domain.Entities;

namespace Connect.Web.ViewModels
{
    public class UserProfileViewModel
    {
        public UserDto User { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<FriendshipDto> Friends { get; set; } = new List<FriendshipDto>();
    }
}