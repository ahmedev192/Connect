using System;

namespace Connect.Application.Dtos
{
    public class FriendRequestDto
    {
        public int RequestId { get; set; }
        public int SenderId { get; set; }
        public string SenderFullName { get; set; }
        public string SenderProfilePictureUrl { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverFullName { get; set; }
        public string ReceiverProfilePictureUrl { get; set; }
        public DateTime SentAt { get; set; }
    }
}