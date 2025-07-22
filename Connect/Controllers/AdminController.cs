using Connect.Controllers.Base;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Connect.Utilities.StaticDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    [Authorize(Roles = ApplicationRoles.Admin)]

    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;
        public AdminController(UserManager<User> userManager, IAdminService adminService) : base(userManager)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()

        {
            var reportedPosts = await _adminService.GetReportedPostsAsync();
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
