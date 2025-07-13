using Connect.DataAccess.Data;
using Connect.Models;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    public class PostController : Controller
    {

        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Create(Post post)
        {
            // This method is used to handle the form submission for creating a new post.

            int userId = 1; // This should be replaced with the actual user ID from the logged-in user context.

           post.UserId = userId; // Set the UserId for the post

            // It should validate the post data and save it to the database.
            if (ModelState.IsValid)
            {
                // Save the post to the database
                _context.Posts.AddAsync(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(post);

        }
    }
}
