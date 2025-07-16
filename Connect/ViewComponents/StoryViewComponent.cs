using System;
using Connect.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Connect.ViewComponents
{
    public class StoryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;


        public StoryViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allStories = await _context.Stories
                            .Where(n => n.DateCreated >= DateTime.UtcNow.AddHours(-24))
                            .Include(s => s.User)
                            .ToListAsync();
            return View(allStories);
        }
    }
}
