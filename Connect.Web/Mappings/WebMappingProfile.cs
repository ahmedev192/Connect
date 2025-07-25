using AutoMapper;
using Connect.Application.Dtos;
using Connect.Domain.Dtos;
using Connect.Domain.Entities;
using Connect.Web.ViewModels;

namespace Connect.Web.Mappings
{
    public class WebMappingProfile : Profile
    {
        public WebMappingProfile()
        {
            // ViewModel to DTO mappings
            CreateMap<RegisterViewModel, RegisterDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.ConfirmPassword, opt => opt.MapFrom(src => src.ConfirmPassword));

            CreateMap<LoginViewModel, LoginDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.RememberMe, opt => opt.MapFrom(src => src.RememberMe));

            CreateMap<UpdateProfileViewModel, UpdateProfileDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio));

            CreateMap<UpdatePasswordViewModel, UpdatePasswordDto>()
                .ForMember(dest => dest.CurrentPassword, opt => opt.MapFrom(src => src.CurrentPassword))
                .ForMember(dest => dest.NewPassword, opt => opt.MapFrom(src => src.NewPassword))
                .ForMember(dest => dest.ConfirmNewPassword, opt => opt.MapFrom(src => src.ConfirmNewPassword));

            // Model to ViewModel mappings
            CreateMap<User, UserProfileViewModel>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Posts, opt => opt.MapFrom(src => src.Posts));

            CreateMap<User, UserWithFriendsCountViewModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl ?? "/images/avatars/user.png"))
                .ForMember(dest => dest.FriendsCount, opt => opt.Ignore());

            // DTO to ViewModel mappings
            CreateMap<UserDto, SettingsViewModel>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.UpdateProfile, opt => opt.MapFrom(src => new UpdateProfileViewModel
                {
                    FullName = src.FullName,
                    UserName = src.UserName,
                    Bio = src.Bio
                }))
                .ForMember(dest => dest.UpdatePassword, opt => opt.Ignore());

            CreateMap<UserProfileDto, UserProfileViewModel>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Posts, opt => opt.MapFrom(src => src.Posts));

            // Map DTO lists to FriendshipsViewModel
            CreateMap<IEnumerable<FriendshipDto>, FriendshipsViewModel>()
                .ForMember(dest => dest.Friends, opt => opt.MapFrom(src => src.ToList()))
                .ForMember(dest => dest.SentRequests, opt => opt.Ignore())
                .ForMember(dest => dest.ReceivedRequests, opt => opt.Ignore());

            CreateMap<IEnumerable<FriendRequestDto>, FriendshipsViewModel>()
                .ForMember(dest => dest.Friends, opt => opt.Ignore())
                .ForMember(dest => dest.SentRequests, opt => opt.MapFrom((src, dest, _, context) =>
                    context.Items.ContainsKey("requestType") && (string)context.Items["requestType"] == "Sent"
                    ? src.ToList() : dest.SentRequests))
                .ForMember(dest => dest.ReceivedRequests, opt => opt.MapFrom((src, dest, _, context) =>
                    context.Items.ContainsKey("requestType") && (string)context.Items["requestType"] == "Received"
                    ? src.ToList() : dest.ReceivedRequests));
        }
    }
}