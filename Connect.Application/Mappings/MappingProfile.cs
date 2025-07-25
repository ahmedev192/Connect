using AutoMapper;
using Connect.Application.Dtos;
using Connect.Domain.Dtos;
using Connect.Domain.Entities;

namespace Connect.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Model to DTO mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl ?? "/images/avatars/user.png"));

            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.DateCreated));

            CreateMap<Friendship, FriendshipDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FriendId, opt => opt.MapFrom((src, dest, _, context) =>
                    context.Items.ContainsKey("currentUserId") && src.User1Id == (int)context.Items["currentUserId"]
                    ? src.User2Id : src.User1Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom((src, dest, _, context) =>
                    context.Items.ContainsKey("currentUserId") && src.User1Id == (int)context.Items["currentUserId"]
                    ? src.User2.FullName : src.User1.FullName))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom((src, dest, _, context) =>
                    context.Items.ContainsKey("currentUserId") && src.User1Id == (int)context.Items["currentUserId"]
                    ? (src.User2.ProfilePictureUrl ?? "/images/avatars/user.png") : (src.User1.ProfilePictureUrl ?? "/images/avatars/user.png")));

            CreateMap<FriendRequest, FriendRequestDto>()
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
                .ForMember(dest => dest.SenderFullName, opt => opt.MapFrom(src => src.Sender.FullName))
                .ForMember(dest => dest.SenderProfilePictureUrl, opt => opt.MapFrom(src => src.Sender.ProfilePictureUrl ?? "/images/avatars/user.png"))
                .ForMember(dest => dest.ReceiverId, opt => opt.MapFrom(src => src.ReceiverId))
                .ForMember(dest => dest.ReceiverFullName, opt => opt.MapFrom(src => src.Receiver.FullName))
                .ForMember(dest => dest.ReceiverProfilePictureUrl, opt => opt.MapFrom(src => src.Receiver.ProfilePictureUrl ?? "/images/avatars/user.png"))
                .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.DateCreated));

            CreateMap<Hashtag, HashtagDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(dest => dest.DateUpdated, opt => opt.MapFrom(src => src.DateUpdated));

            CreateMap<Story, StoryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<Report, ReportDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(dest => dest.DateUpdated, opt => opt.MapFrom(src => src.DateUpdated))
                .ForMember(dest => dest.Success, opt => opt.Ignore())
                .ForMember(dest => dest.SendNotification, opt => opt.Ignore());

            CreateMap<Like, LikeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated));

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(dest => dest.DateUpdated, opt => opt.MapFrom(src => src.DateUpdated))
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<Favorite, FavoriteDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            // DTO to Model mappings
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Posts, opt => opt.Ignore())
                .ForMember(dest => dest.Likes, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.Favorites, opt => opt.Ignore())
                .ForMember(dest => dest.Reports, opt => opt.Ignore())
                .ForMember(dest => dest.Stories, opt => opt.Ignore())
                .ForMember(dest => dest.Notifications, opt => opt.Ignore())
                .ForMember(dest => dest.SentFriendRequests, opt => opt.Ignore())
                .ForMember(dest => dest.ReceivedFriendRequests, opt => opt.Ignore())
                .ForMember(dest => dest.FriendshipsAsUser1, opt => opt.Ignore())
                .ForMember(dest => dest.FriendshipsAsUser2, opt => opt.Ignore());

            CreateMap<UpdateProfileDto, User>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Posts, opt => opt.Ignore())
                .ForMember(dest => dest.Likes, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.Favorites, opt => opt.Ignore())
                .ForMember(dest => dest.Reports, opt => opt.Ignore())
                .ForMember(dest => dest.Stories, opt => opt.Ignore())
                .ForMember(dest => dest.Notifications, opt => opt.Ignore())
                .ForMember(dest => dest.SentFriendRequests, opt => opt.Ignore())
                .ForMember(dest => dest.ReceivedFriendRequests, opt => opt.Ignore())
                .ForMember(dest => dest.FriendshipsAsUser1, opt => opt.Ignore())
                .ForMember(dest => dest.FriendshipsAsUser2, opt => opt.Ignore());
        }
    }
}