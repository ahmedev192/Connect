using System.Collections.Generic;
using System.Threading.Tasks;
using Connect.Application.Dtos;
using Connect.Application.StaticDetails;
using Connect.Domain.Dtos;
using Connect.Domain.Entities;

namespace Connect.Application.Interfaces
{
    public interface IFriendService
    {
        Task<FriendRequest> UpdateRequestAsync(int requestId, FriendRequestStatus newStatus);
        Task SendRequestAsync(int senderId, int receiverId);
        Task RemoveFriendAsync(int friendshipId);
        Task<List<UserWithFriendsCountDto>> GetSuggestedFriendsAsync(int userId);
        Task<List<FriendRequest>> GetSentFriendRequestAsync(int userId);
        Task<List<FriendRequest>> GetReceivedFriendRequestAsync(int userId);
        Task<List<Friendship>> GetFriendsAsync(int userId);
        Task CancelFriendRequestAsync(int requestId, int userId);
    }
}