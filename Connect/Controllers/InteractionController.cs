using System.Threading.Tasks;
using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Utilities.Service;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Connect.Controllers
{
    [Authorize]
    public class InteractionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IPostService _postService;

        public InteractionController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IPostService postService)
        {
            _context = context;
            _userManager = userManager;
            _postService = postService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostLike(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            try
            {
                var post = await _context.Posts
                    .Include(p => p.Likes)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                    return NotFound();

                var existingLike = await _context.Likes
                    .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == user.Id);

                if (existingLike != null)
                    _context.Likes.Remove(existingLike);
                else
                    await _context.Likes.AddAsync(new Like { PostId = postId, UserId = user.Id });

                await _context.SaveChangesAsync();
            }
            catch
            {
                return View();
            }

            var updatedPost = await _postService.GetPostById(postId);
            return PartialView("_Post", updatedPost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            if (!ModelState.IsValid)
                return View(comment);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            comment.UserId = user.Id;
            comment.DateCreated = DateTime.UtcNow;
            comment.DateUpdated = DateTime.UtcNow;
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            var post = await _postService.GetPostById(comment.PostId);
            return PartialView("_Post", post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();
            Comment comment = new() ;
            try
            {
                comment = await _context.Comments.FindAsync(commentId);
                if (comment == null || comment.UserId != user.Id)
                    return NotFound();

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Comment deleted successfully!";
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while deleting the comment.");
            }

            var post = await _postService.GetPostById(comment.PostId);
            return PartialView("_Post", post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostFavorite(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == user.Id);

            if (favorite != null)
                _context.Favorites.Remove(favorite);
            else
                await _context.Favorites.AddAsync(new Favorite { PostId = postId, UserId = user.Id });

            await _context.SaveChangesAsync();

            var post = await _postService.GetPostById(postId);
            return PartialView("_Post", post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPostReport(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            try
            {
                var existingReport = await _context.Reports
                    .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == user.Id);

                if (existingReport != null)
                {
                    ModelState.AddModelError("", "You have already reported this post.");
                    return RedirectToAction("Index", "Home");
                }

                var report = new Report
                {
                    PostId = postId,
                    UserId = user.Id,
                    DateCreated = DateTime.UtcNow
                };
                await _context.Reports.AddAsync(report);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Post reported successfully!";
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while reporting the post.");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}