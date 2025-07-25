using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Connect.Domain.Entities;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<int>
{
    [Required]
    public string FullName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }

    // Navigation properties
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<Story> Stories { get; set; } = new List<Story>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<FriendRequest> SentFriendRequests { get; set; } = new List<FriendRequest>();
    public ICollection<FriendRequest> ReceivedFriendRequests { get; set; } = new List<FriendRequest>();
    public ICollection<Friendship> FriendshipsAsUser1 { get; set; } = new List<Friendship>();
    public ICollection<Friendship> FriendshipsAsUser2 { get; set; } = new List<Friendship>();

    // Navigation property to get all friends
    [NotMapped]
    public IEnumerable<User> Friends
    {
        get
        {
            var friends = new List<User>();
            foreach (var friendship in FriendshipsAsUser1)
            {
                friends.Add(friendship.User2);
            }
            foreach (var friendship in FriendshipsAsUser2)
            {
                friends.Add(friendship.User1);
            }
            return friends;
        }
    }
}