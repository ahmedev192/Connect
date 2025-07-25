using System;

namespace Connect.Application.Dtos
{
    public class FriendshipDto
    {
        public int Id { get; set; }
        public int FriendId { get; set; }
        public string FullName { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}