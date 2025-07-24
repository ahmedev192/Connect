using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect.Application.Dtos
{
    public class FriendRequestDto
    {
        public int RequestId { get; set; }
        public int SenderId { get; set; }
        public string SenderFullName { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverFullName { get; set; }
        public DateTime SentAt { get; set; }
    }
}
