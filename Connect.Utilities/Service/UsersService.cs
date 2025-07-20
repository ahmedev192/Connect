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
    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly IPostService _postService;
        public UsersService(ApplicationDbContext context, IFileUploadService fileUploadService, IPostService postService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _postService = postService;
        }

        public async Task<User> GetUser(int loggedInUserId)
        {
            return await _context.Users.FirstOrDefaultAsync(n => n.Id == loggedInUserId) ?? new User();
        }


        public async Task<List<Post>> GetPostsByUserId(int userId = 0)
        {
            var posts = await _context.Posts
            .Where(p => (userId == 0 || p.UserId == userId) && p.Reports.Count < 5)
            .OrderByDescending(p => p.DateCreated)
            .IncludeAllPostData()
            .ToListAsync();



            return await _postService.ProcessPosts(posts);
        }









    }

}
