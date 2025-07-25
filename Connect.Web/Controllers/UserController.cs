using AutoMapper;
using Connect.Application.Dtos;
using Connect.Application.Interfaces;
using Connect.Application.Service;
using Connect.Application.Services;
using Connect.Controllers.Base;
using Connect.Domain.Entities;
using Connect.Web.Mappings;
using Connect.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUsersService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IFriendService _friendService;
        private readonly IPostService _postService;
        private readonly IMapper _mapper;


        public UserController(IUsersService userService, UserManager<User> userManager, IFriendService friendService, IPostService postService , IMapper mapper) : base(userManager)
        {
            _userService = userService;
            _userManager = userManager;
            _friendService = friendService;
            _postService = postService;
            _mapper = mapper;
        }



        public async Task<IActionResult> UserDetails(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var friends = await _friendService.GetFriendsAsync(userId);
            var posts = await _userService.GetPostsByUserId(userId);

            var viewModel = new UserProfileViewModel
            {
                User = _mapper.Map<UserDto>(user),
                Posts = posts,
                Friends = friends.Select(f => new FriendshipDto
                {
                    Id = f.Id,
                    FriendId = f.User1Id == userId ? f.User2Id : f.User1Id,
                    FullName = f.User1Id == userId ? f.User2.FullName : f.User1.FullName,
                    ProfilePictureUrl = f.User1Id == userId
                        ? (f.User2.ProfilePictureUrl ?? "/images/avatars/user.png")
                        : (f.User1.ProfilePictureUrl ?? "/images/avatars/user.png")
                }).ToList()
            };

            return View(viewModel);
        }
    }
}
