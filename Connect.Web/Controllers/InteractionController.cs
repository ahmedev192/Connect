using System.Threading.Tasks;
using Connect.Controllers.Base;
using Connect.Infrastructure.Data;
using Connect.Infrastructure.Migrations;
using Connect.Domain.Entities;
using Connect.Application.Service;
using Connect.Application.Interfaces;
using Connect.Application.StaticDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Connect.Web.Hubs;

namespace Connect.Controllers
{
    [Authorize]
    public class InteractionController : BaseController
    {
        private readonly IInteractionService _interactionService;
        private readonly IPostService _postService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;
        public InteractionController(
            IInteractionService interactionService,
            IPostService postService,
            UserManager<User> userManager,
            IHubContext<NotificationHub> hubContext, INotificationService notificationService) : base(userManager)
        {
            _interactionService = interactionService;
            _postService = postService;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostLike(int postId)
        {
            var user = GetUserId();
            var userName =  GetUserFullName();
            if (user == null)
                return Unauthorized();

            try
            {
                var result = await _interactionService.TogglePostLikeAsync(postId, user.Value);
                var post = await _postService.GetPostById(postId);


                if (result.SendNotification && user.Value != post.UserId)
                    await _notificationService.AddNewNotificationAsync(post.UserId, NotificationType.Like, userName, postId);
              
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

            var user = GetUserId();
            var userName = GetUserFullName();
            if (user == null)
                return Unauthorized();

            try
            {
                await _interactionService.AddCommentAsync(comment, user.Value);
                var post = await _postService.GetPostById(comment.PostId);
                if (user.Value != post.UserId)
                    await _notificationService.AddNewNotificationAsync(post.UserId, NotificationType.Comment, userName, comment.PostId);

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
            var user = GetUserId();
            if (user == null)
                return Unauthorized();

            try
            {
                var postId = await _interactionService.DeleteCommentAsync(commentId, user.Value);
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
            var user = GetUserId();
            var userName =  GetUserFullName();
            if (user == null)
                return Unauthorized();


            try
            {
                var result = await _interactionService.TogglePostFavoriteAsync(postId, user.Value);
                var post = await _postService.GetPostById(postId);
                if (result.SendNotification && user.Value != post.UserId)
                    await _notificationService.AddNewNotificationAsync(post.UserId, NotificationType.Favorite, userName, postId);


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
            var user = GetUserId();
            if (user == null)
                return Unauthorized();

            try
            {
                await _interactionService.AddPostReportAsync(postId, user.Value);
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