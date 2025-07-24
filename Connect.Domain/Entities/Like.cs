using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Connect.Domain.Entities
{
    public class Like
    {
        public int Id { get; set; }

        public int PostId { get; set; }
        public int UserId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Navigation properties
        [ValidateNever]
        public Post Post { get; set; }
        [ValidateNever]
        public User User { get; set; }

    }
}
