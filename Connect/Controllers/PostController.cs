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



        [HttpPost]
        public async Task<IActionResult> AddPostComment(Comment comment)
        {
            int userId = 1; // Temporary user ID, will be replaced with actual user ID from authentication context

            comment.UserId = userId;
            comment.DateCreated = DateTime.UtcNow;
            comment.DateUpdated = DateTime.UtcNow;
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");



        }


        [HttpPost]
        public async Task<IActionResult> RemovePostComment(int commentId)
        {
            try
            {
                var comment = await _context.Comments.FindAsync(commentId);
                if (comment == null)
                {
                    return NotFound();
                }
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");





        }


        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(int  postId)
        {
            int userId = 1;

            //check if user has already favorited the post
            var favorite = await _context.Favorites
                .Where(l => l.PostId == postId && l.UserId == userId)
                .FirstOrDefaultAsync();

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newFavorite = new Favorite()
                {
                    PostId = postId,
                    UserId = userId
                };
                await _context.Favorites.AddAsync(newFavorite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index" , "Home");
        }







        [HttpPost]
        public async Task<IActionResult> ToggleVisability(int postId)
        {
            int userId = 1; // Temporary user ID, will be replaced with actual user ID from authentication context
            try
            {
                var post = await _context.Posts.FindAsync(postId);
                if (post == null)
                {
                    return NotFound();
                }
                post.IsPrivate = !post.IsPrivate;
                post.DateUpdated = DateTime.UtcNow;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Post visibility updated successfully!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the post visibility.");
            }
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> AddPostReport(int postId)
        {
            int userId = 1; // Temporary user ID, will be replaced with actual user ID from authentication context
            try
            {
                var existingReport = await _context.Reports
                    .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);
                if (existingReport != null)
                {
                    ModelState.AddModelError("", "You have already reported this post.");
                    return RedirectToAction("Index", "Home");
                }
                var report = new Report
                {
                    PostId = postId,
                    UserId = userId,
                    DateCreated = DateTime.UtcNow
                };
                await _context.Reports.AddAsync(report);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Post reported successfully!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while reporting the post.");
            }
            return RedirectToAction("Index", "Home");
        }



        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            int userId = 1; // Temporary user ID, will be replaced with actual user ID from authentication context
            try
            {
                var post = await _context.Posts.FindAsync(postId);
                if (post == null || post.UserId != userId)
                {
                    return NotFound();
                }
                // Remove associated likes, comments, and favorites
                var associatedLikes = _context.Likes.Where(l => l.PostId == postId);
                var associatedFavorites = _context.Favorites.Where(f => f.PostId == postId);
                var associatedReports = _context.Reports.Where(r => r.PostId == postId);
                var associatedComments = _context.Comments.Where(c => c.PostId == postId);
                _context.Likes.RemoveRange(associatedLikes);
                _context.Favorites.RemoveRange(associatedFavorites);
                _context.Reports.RemoveRange(associatedReports);
                _context.Comments.RemoveRange(associatedComments);
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Post deleted successfully!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the post.");
            }
            return RedirectToAction("Index", "Home");
        }


    }
}
