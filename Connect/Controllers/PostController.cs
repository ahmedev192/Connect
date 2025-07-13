using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

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
            int userId = 1; // TODO: Replace with actual user ID

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
                    return RedirectToAction( "Index" , "Home");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the post.");
            }

            return View(post);
        }

    }
}
