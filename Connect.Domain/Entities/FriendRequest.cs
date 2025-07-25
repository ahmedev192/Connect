using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Application.StaticDetails;
using Microsoft.EntityFrameworkCore;

namespace Connect.Domain.Entities
{
    [Index(nameof(SenderId), nameof(ReceiverId), IsUnique = true)]
    public class FriendRequest
    {
        public int Id { get; set; }
        [Required]
        public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;

        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int ReceiverId { get; set; }
        public User Receiver { get; set; }

        public int UniqueSenderId { get => SenderId; set => SenderId = value; }
        public int UniqueReceiverId { get => ReceiverId; set => ReceiverId = value; }
    }
}
