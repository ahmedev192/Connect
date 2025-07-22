using System.Threading.Tasks;
using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Utilities.Service;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    [Authorize]
    public class InteractionController : Controller
    {
        private readonly IInteractionService _interactionService;
        private readonly IPostService _postService;
        private readonly UserManager<User> _userManager;

        public InteractionController(
            IInteractionService interactionService,
            IPostService postService,
            UserManager<User> userManager)
        {
            _interactionService = interactionService;
            _postService = postService;
            _userManager = userManager;
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
                await _interactionService.TogglePostLikeAsync(postId, user.Id);
                var post = await _postService.GetPostById(postId);
                return PartialView("_Post", post);
            }
            catch
            {
                return View();
            }
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

            try
            {
                await _interactionService.AddCommentAsync(comment, user.Id);
                var post = await _postService.GetPostById(comment.PostId);
                return PartialView("_Post", post);
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while adding the comment.");
                return View(comment);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            try
            {
                var postId = await _interactionService.DeleteCommentAsync(commentId, user.Id);
                if (postId == null)
                    return NotFound();

                TempData["Success"] = "Comment deleted successfully!";
                var post = await _postService.GetPostById(postId.Value);
                return PartialView("_Post", post);
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while deleting the comment.");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostFavorite(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            try
            {
                await _interactionService.TogglePostFavoriteAsync(postId, user.Id);
                var post = await _postService.GetPostById(postId);
                return PartialView("_Post", post);
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while toggling favorite.");
                return View();
            }
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
                await _interactionService.AddPostReportAsync(postId, user.Id);
                TempData["Success"] = "Post reported successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while reporting the post.");
                return RedirectToAction("Index", "Home");
            }
        }
    }
}