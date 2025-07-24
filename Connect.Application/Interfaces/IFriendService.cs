using Connect.Domain.Dtos;
using Connect.Domain.Entities;

namespace Connect.Application.Interfaces
{
    public interface IFriendService
    {
        Task SendRequestAsync(int senderId, int receiverId);
        Task<FriendRequest> UpdateRequestAsync(int requestId, string status);
        Task RemoveFriendAsync(int frienshipId);
        Task<List<UserWithFriendsCountDto>> GetSuggestedFriendsAsync(int userId);
        Task<List<FriendRequest>> GetSentFriendRequestAsync(int userId);
        Task<List<FriendRequest>> GetReceivedFriendRequestAsync(int userId);
        Task<List<Friendship>> GetFriendsAsync(int userId);






    }
}
