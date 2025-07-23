using System.Linq.Expressions;
using System.Threading.Tasks;
using Connect.Infrastructure.Data;
using Connect.Infrastructure.Repository;
using Connect.Infrastructure.Repository.IRepository;
using Connect.Domain;
using Connect.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Service
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;
        private readonly IHashtagService _hashtagService;
        private readonly INotificationService _notificationService;

        public PostService(
            IUnitOfWork unitOfWork,
            IFileUploadService fileUploadService,
            IHashtagService hashtagService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
            _hashtagService = hashtagService;
            _notificationService = notificationService;
        }

        public async Task<Post> GetPostById(int postId)
        {
            var post = await _unitOfWork.PostRepository.Query(noTracking: true)
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
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.PostRepository.AddAsync(post);

                var postHashtags = _hashtagService.ExtractHashtags(post.Content);
                foreach (var hashTag in postHashtags)
                {
                    var hashtag = await _unitOfWork.HashtagRepository.FirstOrDefaultAsync(h => h.Name == hashTag);
                    if (hashtag != null)
                    {
                        hashtag.Count += 1;
                        hashtag.DateUpdated = DateTime.UtcNow;
                        _unitOfWork.HashtagRepository.Update(hashtag);
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
                        await _unitOfWork.HashtagRepository.AddAsync(newHashtag);
                    }
                }
            });
        }

        public async Task<ServiceResult> DeletePostAsync(int postId, int userId)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(postId);
            if (post == null || post.UserId != userId)
                return new ServiceResult { Succeeded = false, ErrorMessage = "Post not found or unauthorized." };

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                if (!string.IsNullOrEmpty(post.ImageUrl))
                    await _fileUploadService.DeleteImageAsync(post.ImageUrl);

                var postHashtags = _hashtagService.ExtractHashtags(post.Content);
                foreach (var hashtag in postHashtags)
                {
                    var hashtagDb = await _unitOfWork.HashtagRepository.FirstOrDefaultAsync(h => h.Name == hashtag);
                    if (hashtagDb != null)
                    {
                        hashtagDb.Count -= 1;
                        hashtagDb.DateUpdated = DateTime.UtcNow;
                        _unitOfWork.HashtagRepository.Update(hashtagDb);
                    }
                }

                var associatedLikes = await _unitOfWork.LikeRepository.FindAsync(l => l.PostId == postId);
                var associatedFavorites = await _unitOfWork.FavoriteRepository.FindAsync(f => f.PostId == postId);
                var associatedReports = await _unitOfWork.ReportRepository.FindAsync(r => r.PostId == postId);
                var associatedComments = await _unitOfWork.CommentRepository.FindAsync(c => c.PostId == postId);

                if (associatedLikes.Any())
                    _unitOfWork.LikeRepository.RemoveRange(associatedLikes);
                if (associatedFavorites.Any())
                    _unitOfWork.FavoriteRepository.RemoveRange(associatedFavorites);
                if (associatedReports.Any())
                    _unitOfWork.ReportRepository.RemoveRange(associatedReports);
                if (associatedComments.Any())
                    _unitOfWork.CommentRepository.RemoveRange(associatedComments);

                _unitOfWork.PostRepository.Remove(post);
            });

            return new ServiceResult { Succeeded = true };
        }


    }
}