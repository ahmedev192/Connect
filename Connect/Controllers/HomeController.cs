using System;
using System.Diagnostics;
using Connect.Controllers.Base;
using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Connect.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IUsersService _userService;



        public HomeController( IUsersService usersService, UserManager<User> userManager) : base(userManager)
        {
            _userService = usersService;
        }

        public async Task<IActionResult> Index()
        {
            var user = GetUserId();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var allPosts =await  _userService.GetPostsByUserId(0);

            return View(allPosts);
        }






    }
}
