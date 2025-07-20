using Connect.Models;

namespace Connect.DTOs
{
    public class UserWithFriendsCountDTO
    {
        public User User { get; set; }
        public int FriendsCount { get; set; }
    }
}
