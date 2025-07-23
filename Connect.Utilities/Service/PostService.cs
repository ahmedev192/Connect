using System.Linq.Expressions;
using System.Threading.Tasks;
using Connect.DataAccess.Data;
using Connect.DataAccess.Repository.IRepository;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace Connect.Utilities.Service
{
    public static class QueryablePostExtensions
    {
        public static IQueryable<Post> IncludeAllPostData(this IQueryable<Post> query)
        {
            return query
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Favorites)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Reports);
        }
    }

    public class PostService : IPostService
    {
        private readonly IGenericRepository<Post> _postRepository;
        private readonly IGenericRepository<Hashtag> _hashtagRepository;
        private readonly IGenericRepository<Like> _likeRepository;
        private readonly IGenericRepository<Favorite> _favoriteRepository;
        private readonly IGenericRepository<Report> _reportRepository;
        private readonly IGenericRepository<Comment> _commentRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IHashtagService _hashtagService;
        private readonly INotificationService _notificationService;

        public PostService(
            IGenericRepository<Post> postRepository,
            IGenericRepository<Hashtag> hashtagRepository,
            IGenericRepository<Like> likeRepository,
            IGenericRepository<Favorite> favoriteRepository,
            IGenericRepository<Report> reportRepository,
            IGenericRepository<Comment> commentRepository,
            IFileUploadService fileUploadService,
            IHashtagService hashtagService,
            INotificationService notificationService)
        {
            _postRepository = postRepository;
            _hashtagRepository = hashtagRepository;
            _likeRepository = likeRepository;
            _favoriteRepository = favoriteRepository;
            _reportRepository = reportRepository;
            _commentRepository = commentRepository;
            _fileUploadService = fileUploadService;
            _hashtagService = hashtagService;
            _notificationService = notificationService;
        }

        public async Task<Post> GetPostById(int postId)
        {
            var post = await _postRepository.Query(noTracking: true)
                .IncludeAllPostData()
                .FirstOrDefaultAsync(p => p.Id == postId);

            return await ProcessPosts(post);
        }

        public async Task<List<Post>> ProcessPosts(IEnumerable<Post> posts)
        {
            if (posts == null || !posts.Any())
            {
                return new List<Post>();
            }

            var processedPosts = new List<Post>();

            foreach (var post in posts)
            {
                if (!string.IsNullOrEmpty(post.ImageUrl))
                {
                    post.ImageUrl = _fileUploadService.ResolveImageOrDefault(
                        post.ImageUrl,
                        "/images/placeholders/post-placeholder.jpg"
                    );
                }

                if (post.User != null)
                {
                    post.User.ProfilePictureUrl = _fileUploadService.ResolveImageOrDefault(
                        post.User.ProfilePictureUrl,
                        "/images/avatars/user.png"
                    );
                }

                processedPosts.Add(post);
            }

            return processedPosts;
        }

        public async Task<Post> ProcessPosts(Post post)
        {
            if (post == null)
            {
                return null;
            }

            var posts = new List<Post> { post };
            await ProcessPosts(posts);
            return posts.FirstOrDefault();
        }

        public async Task CreatePostAsync(Post post)
        {
            await _postRepository.AddAsync(post);

            var postHashtags = _hashtagService.ExtractHashtags(post.Content);
            foreach (var hashTag in postHashtags)
            {
                var hashtag = await _hashtagRepository.FirstOrDefaultAsync(h => h.Name == hashTag);
                if (hashtag != null)
                {
                    hashtag.Count += 1;
                    hashtag.DateUpdated = DateTime.UtcNow;
                    _hashtagRepository.Update(hashtag);
                }
                else
                {
                    var newHashtag = new Hashtag
                    {
                        Name = hashTag,
                        Count = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    };
                    await _hashtagRepository.AddAsync(newHashtag);
                }
            }
        }

        public async Task<ServiceResult> DeletePostAsync(int postId, int userId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null || post.UserId != userId)
                return new ServiceResult { Succeeded = false, ErrorMessage = "Post not found or unauthorized." };

            if (!string.IsNullOrEmpty(post.ImageUrl))
                await _fileUploadService.DeleteImageAsync(post.ImageUrl);

            var postHashtags = _hashtagService.ExtractHashtags(post.Content);
            foreach (var hashtag in postHashtags)
            {
                var hashtagDb = await _hashtagRepository.FirstOrDefaultAsync(h => h.Name == hashtag);
                if (hashtagDb != null)
                {
                    hashtagDb.Count -= 1;
                    hashtagDb.DateUpdated = DateTime.UtcNow;
                    _hashtagRepository.Update(hashtagDb);
                }
            }

            var associatedLikes = await _likeRepository.FindAsync(l => l.PostId == postId);
            var associatedFavorites = await _favoriteRepository.FindAsync(f => f.PostId == postId);
            var associatedReports = await _reportRepository.FindAsync(r => r.PostId == postId);
            var associatedComments = await _commentRepository.FindAsync(c => c.PostId == postId);

            await _postRepository.ExecuteInTransactionAsync(async () =>
            {
                if (associatedLikes.Any())
                    _likeRepository.RemoveRange(associatedLikes);
                if (associatedFavorites.Any())
                    _favoriteRepository.RemoveRange(associatedFavorites);
                if (associatedReports.Any())
                    _reportRepository.RemoveRange(associatedReports);
                if (associatedComments.Any())
                    _commentRepository.RemoveRange(associatedComments);

                _postRepository.Remove(post);
            });

            return new ServiceResult { Succeeded = true };
        }
    }
}