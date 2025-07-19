using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IUsersService _userService;
        private readonly IPostService _postService;

        public PostController(ApplicationDbContext context, IFileUploadService fileUploadService, IHashtagService hashtagService, UserManager<User> userManager, IUsersService usersService, IPostService postService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _hashtagService = hashtagService;
            _userManager = userManager;
            _userService = usersService;
            _postService = postService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {


            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Post post, IFormFile? file)
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
                    if (file != null && !file.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("Image", "Only image files are allowed.");
                        return View(post);
                    }

                    string? imageUrl = await _fileUploadService.SaveImageAsync(file, "posts");

                    post.UserId = userId;
                    post.ImageUrl = imageUrl ?? "";

                    await _context.Posts.AddAsync(post);
                    await _context.SaveChangesAsync();





                    //Find and store hashtags
                    var postHashtags = _hashtagService.ExtractHashtags(post.Content);
                    foreach (var hashTag in postHashtags)
                    {
                        var hashtagDb = await _context.Hashtags.FirstOrDefaultAsync(n => n.Name == hashTag);
                        if (hashtagDb != null)
                        {
                            hashtagDb.Count += 1;
                            hashtagDb.DateUpdated = DateTime.UtcNow;

                            _context.Hashtags.Update(hashtagDb);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            var newHashtag = new Hashtag()
                            {
                                Name = hashTag,
                                Count = 1,
                                DateCreated = DateTime.UtcNow,
                                DateUpdated = DateTime.UtcNow
                            };
                            await _context.Hashtags.AddAsync(newHashtag);
                            await _context.SaveChangesAsync();
                        }
                    }







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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            int userId = user.Id;

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
            if (!ModelState.IsValid)
            {
                return View(comment);
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            int userId = user.Id;

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
        public async Task<IActionResult> TogglePostFavorite(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            int userId = user.Id;

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

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> ToggleVisability(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            int userId = user.Id;
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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            int userId = user.Id;
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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            int userId = user.Id;

            try
            {
                var post = await _context.Posts.FindAsync(postId);
                if (post == null || post.UserId != userId)
                {
                    return NotFound();
                }

                //  Delete post image from server
                if (!string.IsNullOrEmpty(post.ImageUrl))
                {
                    await _fileUploadService.DeleteImageAsync(post.ImageUrl);
                }

                //  Update hashtag counts
                var postHashtags = _hashtagService.ExtractHashtags(post.Content);
                foreach (var hashtag in postHashtags)
                {
                    var hashtagDb = await _context.Hashtags.FirstOrDefaultAsync(n => n.Name == hashtag);
                    if (hashtagDb != null)
                    {
                        hashtagDb.Count -= 1;
                        hashtagDb.DateUpdated = DateTime.UtcNow;

                        _context.Hashtags.Update(hashtagDb);
                        await _context.SaveChangesAsync();
                    }
                }

                //  Remove associated entities
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



        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            int userId = user.Id;
            try
            {
                var comment = await _context.Comments.FindAsync(commentId);
                if (comment == null || comment.UserId != userId)
                {
                    return NotFound();
                }
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Comment deleted successfully!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the comment.");
            }
            return RedirectToAction("Index", "Home");

        }


        [HttpGet]
        public async Task<IActionResult> GetAllFavoritePosts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            int userId = user.Id;
            var favoritePosts = await _context.Favorites
              .Where(f => f.UserId == userId).Include(u => u.User).Include(m => m.Post).ToListAsync();

            return View(favoritePosts);

        }


        [HttpGet]
        public async Task<IActionResult> PostDetails(int postId)
        {
            var post = await  _postService.GetPostById(postId);
            return View(post);

        }




    }
}
