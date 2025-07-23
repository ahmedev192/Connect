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
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;
        private readonly IPostService _postService;

        public UsersService(
            IUnitOfWork unitOfWork,
            IFileUploadService fileUploadService,
            IPostService postService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
            _postService = postService;
        }

        public async Task<User> GetUser(int loggedInUserId)
        {
            return await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.Id == loggedInUserId, noTracking: true) ?? new User();
        }

        public async Task<List<Post>> GetPostsByUserId(int userId = 0)
        {
            var posts = (await _unitOfWork.PostRepository.GetPagedAsync(
               page: 1,
               pageSize: int.MaxValue,
               orderBy: p => p.DateCreated,
               descending: true,
               predicate: p => (userId == 0 || p.UserId == userId) && p.Reports.Count < 5,
               noTracking: true,
               p => p.User,
               p => p.Likes,
               p => p.Favorites,
               p => p.Comments,
               p => p.Reports
           )).ToList();


            // Handle ThenInclude for Comments.User
            var postsWithComments = await _unitOfWork.PostRepository.Query(noTracking: true)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Where(p => (userId == 0 || p.UserId == userId) && p.Reports.Count < 5)
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();

            // Merge the results
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