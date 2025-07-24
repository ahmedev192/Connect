using Connect.Application.Dtos;
using Connect.Domain.Entities;
using Connect.Domain.Entities;

namespace Connect.Domain.Dtos
{
    public class UserWithFriendsCountDto
    {
        public User User { get; set; }
        public int FriendsCount { get; set; }
    }
}
