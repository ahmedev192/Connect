using System;
using System.Diagnostics;
using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Connect.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IFileUploadService fileUploadService)
        {
            _logger = logger;
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var allPosts = await _context.Posts.Where(post => !post.IsPrivate)
                .Include(post => post.User)
                .Include(post => post.Likes)
                .Include(post => post.Comments)
                    .ThenInclude(comment => comment.User).Include(u => u.Favorites).Include(u => u.Reports)
                .OrderByDescending(post => post.DateCreated)
                .ToListAsync();
            foreach (var post in allPosts)
            {
                if (!string.IsNullOrEmpty(post.ImageUrl)) { 
                    post.ImageUrl = _fileUploadService.ResolveImageOrDefault(post.ImageUrl, "/images/placeholders/post-placeholder.jpg");
                }
            }

            return View(allPosts);
        }






    }
}
