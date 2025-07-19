using System;
using Connect.DataAccess.Data;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Connect.ViewComponents
{
    public class StoryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;


        public StoryViewComponent(ApplicationDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allStories = await _context.Stories
                            .Where(n => n.DateCreated >= DateTime.UtcNow.AddHours(-24))
                            .Include(s => s.User)
                            .ToListAsync();
            foreach (var story in allStories)
            {
                story.User.ProfilePictureUrl =  _fileUploadService.ResolveImageOrDefault(story.User.ProfilePictureUrl, "/images/avatars/user.png");
            }
            return View(allStories);
        }
    }
}
