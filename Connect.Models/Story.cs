using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Connect.Models
{
    public class Story
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Foreign key
        public int UserId { get; set; }
        [ValidateNever]
        public User User { get; set; }
    }
}
