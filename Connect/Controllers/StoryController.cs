using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Connect.Controllers
{
    public class StoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public StoryController(ApplicationDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }




        [HttpPost]
        public async Task<IActionResult> CreateStory(Story story, IFormFile file)
        {

            int userId = 1; // Temrary user ID, will be replaced with actual user ID from authentication context
            try
            {
                if (ModelState.IsValid)
                {

                    string? imageUrl = await _fileUploadService.SavePostImageAsync(file, "stories");

                    story.UserId = userId;
                    story.ImageUrl = imageUrl ?? "";

                    await _context.Stories.AddAsync(story);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index" , "Home");

                }
                else
                {
                    ModelState.AddModelError("", "Invalid model state");
                }




            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while creating the story: {ex.Message}");
            }
            return View(story);

        }
    }
}
