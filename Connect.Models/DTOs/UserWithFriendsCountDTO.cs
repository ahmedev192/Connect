using Connect.Models;

namespace Connect.Models.DTOs
{
    public class UserWithFriendsCountDTO
    {
        public User User { get; set; }
        public int FriendsCount { get; set; }
    }
}
