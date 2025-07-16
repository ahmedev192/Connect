using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Connect.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public int PostId { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        [ValidateNever]
        public Post Post { get; set; }
        [ValidateNever]

        public User User { get; set; }
    }
}
