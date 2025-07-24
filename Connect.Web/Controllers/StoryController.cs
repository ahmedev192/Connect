using Connect.Infrastructure.Data;
using Connect.Domain.Entities;
using Connect.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Connect.Controllers
{
    [Authorize]
    public class StoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly UserManager<User> _userManager; 
        public StoryController(ApplicationDbContext context, IFileUploadService fileUploadService , UserManager<User> userManager)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _userManager = userManager;
        }




        [HttpPost]
        public async Task<IActionResult> CreateStory(Story story, IFormFile file)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            int userId = user.Id;
            try
            {
                if (ModelState.IsValid)
                {

                    string? imageUrl = await _fileUploadService.SaveImageAsync(file, "stories");

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
