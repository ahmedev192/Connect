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
        private readonly UserManager<User> _userManager;
        public AdminController(UserManager<User> userManager, IAdminService adminService, IFileUploadService fileUploadService) : base(userManager)
        {
            _adminService = adminService;
            _fileUploadService = fileUploadService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var reportedPosts = await _adminService.GetReportedPostsAsync();
            var users = _userManager.Users.ToList();

            var model = new
            {
                Posts = reportedPosts,
                Users = users
            };

            return View(model);
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
