namespace Connect.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Navigation property for the one-to-many relationship
        public ICollection<Post> Posts { get; set; } = new List<Post>();

    }
}
