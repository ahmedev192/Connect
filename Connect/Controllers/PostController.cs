using System.Threading.Tasks;
using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Connect.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly IHashtagService _hashtagService;
        private readonly UserManager<User> _userManager;
        private readonly IPostService _postService;

        public PostController(
            ApplicationDbContext context,
            IFileUploadService fileUploadService,
            IHashtagService hashtagService,
            UserManager<User> userManager,
            IPostService postService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _hashtagService = hashtagService;
            _userManager = userManager;
            _postService = postService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Post post, IFormFile? file)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null && !file.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("Image", "Only image files are allowed.");
                        return View(post);
                    }

                    string? imageUrl = await _fileUploadService.SaveImageAsync(file, "posts");
                    post.UserId = user.Id;
                    post.ImageUrl = imageUrl ?? "";
                    await _postService.CreatePostAsync(post);

                    TempData["Success"] = "Post created successfully!";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while creating the post.");
            }

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleVisibility(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            try
            {
                var post = await _context.Posts.FindAsync(postId);
                if (post == null || post.UserId != user.Id)
                    return NotFound();

                post.IsPrivate = !post.IsPrivate;
                post.DateUpdated = DateTime.UtcNow;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Post visibility updated successfully!";
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while updating the post visibility.");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            try
            {
                var result = await _postService.DeletePostAsync(postId, user.Id);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Post deleted successfully!";
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", result.ErrorMessage);
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while deleting the post.");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> PostDetails(int postId)
        {
            var post = await _postService.GetPostById(postId);
            if (post == null)
                return NotFound();

            return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFavoritePosts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var favoritePosts = await _context.Favorites
                .Where(f => f.UserId == user.Id)
                .Include(u => u.User)
                .ToListAsync();

            foreach (var favoritePost in favoritePosts)
            {
                favoritePost.Post = await _postService.GetPostById(favoritePost.PostId);
            }

            return View(favoritePosts);
        }
    }
}