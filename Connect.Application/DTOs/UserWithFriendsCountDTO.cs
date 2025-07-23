using Connect.Domain;

namespace Connect.Domain.DTOs
{
    public class UserWithFriendsCountDTO
    {
        public User User { get; set; }
        public int FriendsCount { get; set; }
    }
}
