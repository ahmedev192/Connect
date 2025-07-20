using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace Connect.Utilities.Service
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly IHashtagService _hashtagService;

        public PostService(ApplicationDbContext context, IFileUploadService fileUploadService, IHashtagService hashtagService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _hashtagService = hashtagService;
        }

        public async Task<Post> GetPostById(int postId)
        {
            var post = await _context.Posts
                .Where(p => p.Id == postId)
                .IncludeAllPostData()
                .FirstOrDefaultAsync();

            return await ProcessPosts(post);
        }

        public async Task<List<Post>> ProcessPosts(List<Post> posts)
        {
            if (posts == null || !posts.Any())
            {
                return posts ?? new List<Post>();
            }

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
            }

            return posts;
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
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            var postHashtags = _hashtagService.ExtractHashtags(post.Content);
            foreach (var hashTag in postHashtags)
            {
                var hashtagDb = await _context.Hashtags.FirstOrDefaultAsync(n => n.Name == hashTag);
                if (hashtagDb != null)
                {
                    hashtagDb.Count += 1;
                    hashtagDb.DateUpdated = DateTime.UtcNow;
                    _context.Hashtags.Update(hashtagDb);
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
                    await _context.Hashtags.AddAsync(newHashtag);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ServiceResult> DeletePostAsync(int postId, int userId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null || post.UserId != userId)
                return new ServiceResult { Succeeded = false, ErrorMessage = "Post not found or unauthorized." };

            if (!string.IsNullOrEmpty(post.ImageUrl))
                await _fileUploadService.DeleteImageAsync(post.ImageUrl);

            var postHashtags = _hashtagService.ExtractHashtags(post.Content);
            foreach (var hashtag in postHashtags)
            {
                var hashtagDb = await _context.Hashtags.FirstOrDefaultAsync(n => n.Name == hashtag);
                if (hashtagDb != null)
                {
                    hashtagDb.Count -= 1;
                    hashtagDb.DateUpdated = DateTime.UtcNow;
                    _context.Hashtags.Update(hashtagDb);
                    await _context.SaveChangesAsync();
                }
            }

            var associatedLikes = _context.Likes.Where(l => l.PostId == postId);
            var associatedFavorites = _context.Favorites.Where(f => f.PostId == postId);
            var associatedReports = _context.Reports.Where(r => r.PostId == postId);
            var associatedComments = _context.Comments.Where(c => c.PostId == postId);

            _context.Likes.RemoveRange(associatedLikes);
            _context.Favorites.RemoveRange(associatedFavorites);
            _context.Reports.RemoveRange(associatedReports);
            _context.Comments.RemoveRange(associatedComments);

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return new ServiceResult { Succeeded = true };
        }


    }

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
}
