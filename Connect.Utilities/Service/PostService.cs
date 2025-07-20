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

        public PostService(ApplicationDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
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
