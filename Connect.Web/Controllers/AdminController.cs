using Connect.Controllers.Base;
using Connect.Application.Interfaces;
using Connect.Application.StaticDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Connect.Domain.Entities;

namespace Connect.Controllers
{
    [Authorize(Roles = ApplicationRoles.Admin)]

    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;
        private readonly IFileUploadService _fileUploadService; 
        public AdminController(UserManager<User> userManager, IAdminService adminService, IFileUploadService fileUploadService) : base(userManager)
        {
            _adminService = adminService;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()

        {
            var reportedPosts = await _adminService.GetReportedPostsAsync();
            //foreach(var post in reportedPosts)
            //{
            //    post.ImageUrl = _fileUploadService.ResolveImageOrDefault(post.ImageUrl, @"wwwroot/images/placeholders/post-placeholder.jpg");
            //}
            return View(reportedPosts);
        }



        [HttpPost]
        public async Task<IActionResult> ApproveReport(int postId)
        {
            await _adminService.ApproveReportAsync(postId);
            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> RejectReport(int postId)
        {
            await _adminService.RejectReportAsync(postId);
            return RedirectToAction("Index");
        }
    }
}
