using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Connect.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Content { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }= DateTime.Now;

        //Foreign keys
        public int PostId { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        [ValidateNever]
        public Post Post { get; set; }
        [ValidateNever]
        public User User { get; set; }
    }
}
