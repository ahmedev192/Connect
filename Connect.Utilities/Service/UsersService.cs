using System.Linq.Expressions;
using System.Threading.Tasks;
using Connect.DataAccess.Data;
using Connect.DataAccess.Repository.IRepository;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace Connect.Utilities.Service
{
    public class UsersService : IUsersService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Post> _postRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IPostService _postService;

        public UsersService(
            IGenericRepository<User> userRepository,
            IGenericRepository<Post> postRepository,
            IFileUploadService fileUploadService,
            IPostService postService)
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _fileUploadService = fileUploadService;
            _postService = postService;
        }

        public async Task<User> GetUser(int loggedInUserId)
        {
            return await _userRepository.FirstOrDefaultAsync(u => u.Id == loggedInUserId, noTracking: true) ?? new User();
        }

        public async Task<List<Post>> GetPostsByUserId(int userId = 0)
        {
            var posts = await _postRepository.GetPagedAsync(
                page: 1,
                pageSize: int.MaxValue, // Fetch all posts
                orderBy: p => p.DateCreated,
                descending: true,
                predicate: p => (userId == 0 || p.UserId == userId) && p.Reports.Count < 5,
                noTracking: true,
                p => p.User,
                p => p.Likes,
                p => p.Favorites,
                p => p.Comments, // Note: ThenInclude is handled separately
                p => p.Reports);
           posts =  posts.ToList();

            // Handle ThenInclude for Comments.User
            var postsWithComments = await _postRepository.Query(noTracking: true)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Where(p => (userId == 0 || p.UserId == userId) && p.Reports.Count < 5)
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();

            // Merge the results (since EF Core doesn't support ThenInclude in includeProperties directly)
            foreach (var post in posts)
            {
                var postWithComments = postsWithComments.FirstOrDefault(p => p.Id == post.Id);
                if (postWithComments != null)
                {
                    post.Comments = postWithComments.Comments;
                }
            }

            return await _postService.ProcessPosts(posts);
        }
    }
}