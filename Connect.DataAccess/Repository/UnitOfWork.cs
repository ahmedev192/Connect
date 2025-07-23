using System;
using System.Threading.Tasks;
using Connect.DataAccess.Data;
using Connect.DataAccess.Repository.IRepository;
using Connect.Models;

namespace Connect.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private bool _disposed = false;

        public IGenericRepository<Post> PostRepository { get; private set; }
        public IGenericRepository<User> UserRepository { get; private set; }
        public IGenericRepository<FriendRequest> FriendRequestRepository { get; private set; }
        public IGenericRepository<Friendship> FriendshipRepository { get; private set; }
        public IGenericRepository<Like> LikeRepository { get; private set; }
        public IGenericRepository<Comment> CommentRepository { get; private set; }
        public IGenericRepository<Favorite> FavoriteRepository { get; private set; }
        public IGenericRepository<Report> ReportRepository { get; private set; }
        public IGenericRepository<Hashtag> HashtagRepository { get; private set; }
        public IGenericRepository<Notification> NotificationRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            PostRepository = new GenericRepository<Post>(context);
            UserRepository = new GenericRepository<User>(context);
            FriendRequestRepository = new GenericRepository<FriendRequest>(context);
            FriendshipRepository = new GenericRepository<Friendship>(context);
            LikeRepository = new GenericRepository<Like>(context);
            CommentRepository = new GenericRepository<Comment>(context);
            FavoriteRepository = new GenericRepository<Favorite>(context);
            ReportRepository = new GenericRepository<Report>(context);
            HashtagRepository = new GenericRepository<Hashtag>(context);
            NotificationRepository = new GenericRepository<Notification>(context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await action();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}