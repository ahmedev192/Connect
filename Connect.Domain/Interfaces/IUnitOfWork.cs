using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Domain;

namespace Connect.Infrastructure.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Post> PostRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<FriendRequest> FriendRequestRepository { get; }
        IGenericRepository<Friendship> FriendshipRepository { get; }
        IGenericRepository<Like> LikeRepository { get; }
        IGenericRepository<Comment> CommentRepository { get; }
        IGenericRepository<Favorite> FavoriteRepository { get; }
        IGenericRepository<Report> ReportRepository { get; }
        IGenericRepository<Hashtag> HashtagRepository { get; }
        IGenericRepository<Notification> NotificationRepository { get; }

        Task<int> SaveChangesAsync();
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
