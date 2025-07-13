using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Connect.Controllers
{
    public class PostController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public PostController(ApplicationDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // This method is used to display the page for creating a new post.


            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Post post, IFormFile? file)
        {
            int userId = 1; // Temrary user ID, will be replaced with actual user ID from authentication context

            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null && !file.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("Image", "Only image files are allowed.");
                        return View(post);
                    }

                    string? imageUrl = await _fileUploadService.SavePostImageAsync(file, "posts");

                    post.UserId = userId;
                    post.ImageUrl = imageUrl ?? "";

                    await _context.Posts.AddAsync(post);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Post created successfully!";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the post.");
            }

            return View(post);
        }



        [HttpPost]
        public async Task<IActionResult> TogglePostLike(int postId)
        {
            int userId = 1; // Temrary user ID, will be replaced with actual user ID from authentication context

            try
            {
                var post = await _context.Posts
                    .Include(p => p.Likes)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    return NotFound();
                }
                var existingLike = await _context.Likes
                    .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);
                if (existingLike != null)
                {
                    _context.Likes.Remove(existingLike);
                }
                else
                {
                    var newLike = new Like { PostId = postId, UserId = userId };
                    await _context.Likes.AddAsync(newLike);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return View();



            }
            return RedirectToAction("Index", "Home");


        }
    }
}
