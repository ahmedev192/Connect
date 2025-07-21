using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Connect.Models
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }
        // Navigation property for the one-to-many relationship
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();

        public ICollection<Story> Stories { get; set; } = new List<Story>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();


    }
}
