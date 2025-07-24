using Connect.Domain.Entities;

namespace Connect.Web.ViewModels
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public List<Post> Posts { get; set; }
    }
}
