using System.Linq.Expressions;
using System.Threading.Tasks;
using Connect.DataAccess.Data;
using Connect.DataAccess.Repository.IRepository;
using Connect.Models;
using Connect.Models.DTOs;
using Connect.Utilities.Service.IService;
using Connect.Utilities.StaticDetails;
using Microsoft.EntityFrameworkCore;

namespace Connect.Utilities.Service
{
    public class FriendService : IFriendService
    {
        private readonly IGenericRepository<FriendRequest> _friendRequestRepository;
        private readonly IGenericRepository<Friendship> _friendshipRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IFileUploadService _fileUploadService;

        public FriendService(
            IGenericRepository<FriendRequest> friendRequestRepository,
            IGenericRepository<Friendship> friendshipRepository,
            IGenericRepository<User> userRepository,
            IFileUploadService fileUploadService)
        {
            _friendRequestRepository = friendRequestRepository;
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
            _fileUploadService = fileUploadService;
        }

        public async Task<FriendRequest> UpdateRequestAsync(int requestId, string newStatus)
        {
            var request = await _friendRequestRepository.GetByIdAsync(requestId);
            if (request != null)
            {
                request.Status = newStatus;
                request.DateUpdated = DateTime.UtcNow;
                _friendRequestRepository.Update(request);

                if (newStatus == FriendshipStatus.Accepted)
                {
                    var friendship = new Friendship
                    {
                        SenderId = request.SenderId,
                        ReceiverId = request.ReceiverId,
                        DateCreated = DateTime.UtcNow
                    };
                    await _friendshipRepository.AddAsync(friendship);
                }
            }
            return request;
        }

        public async Task SendRequestAsync(int senderId, int receiverId)
        {
            var request = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = FriendshipStatus.Pending,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };
            await _friendRequestRepository.AddAsync(request);
        }

        public async Task RemoveFriendAsync(int friendshipId)
        {
            var friendship = await _friendshipRepository.GetByIdAsync(friendshipId);
            if (friendship != null)
            {
                _friendshipRepository.Remove(friendship);

                var requests = await _friendRequestRepository.FindAsync(
                    r => (r.SenderId == friendship.SenderId && r.ReceiverId == friendship.ReceiverId) ||
                         (r.SenderId == friendship.ReceiverId && r.ReceiverId == friendship.SenderId));
                if (requests.Any())
                {
                    _friendRequestRepository.RemoveRange(requests);
                }
            }
        }

        public async Task<List<UserWithFriendsCountDTO>> GetSuggestedFriendsAsync(int userId)
        {
            var existingFriendIds = await _friendshipRepository.SelectAsync(
                f => f.SenderId == userId ? f.ReceiverId : f.SenderId,
                f => f.SenderId == userId || f.ReceiverId == userId,
                noTracking: true);

            var pendingRequestIds = await _friendRequestRepository.SelectAsync(
                r => r.SenderId == userId ? r.ReceiverId : r.SenderId,
                r => (r.SenderId == userId || r.ReceiverId == userId) && r.Status == FriendshipStatus.Pending,
                noTracking: true);

            var suggestedFriends = await _userRepository.TakeAsync(
                count: 5,
                predicate: u => u.Id != userId && !existingFriendIds.Contains(u.Id) && !pendingRequestIds.Contains(u.Id),
                noTracking: true);

            return suggestedFriends.Select(u => new UserWithFriendsCountDTO
            {
                User = u,
                FriendsCount = _friendshipRepository.CountAsync(f => f.SenderId == u.Id || f.ReceiverId == u.Id).Result
            }).ToList();
        }

        public async Task<List<FriendRequest>> GetSentFriendRequestAsync(int userId)
        {
            var friendRequests = await _friendRequestRepository.FindAsync(
                f => f.SenderId == userId && f.Status == FriendshipStatus.Pending,
                noTracking: true,
                f => f.Sender,
                f => f.Receiver);

            foreach (var friendRequest in friendRequests)
            {
                friendRequest.Sender.ProfilePictureUrl = _fileUploadService.ResolveImageOrDefault(
                    friendRequest.Sender.ProfilePictureUrl, "/images/avatars/user.png");
                friendRequest.Receiver.ProfilePictureUrl = _fileUploadService.ResolveImageOrDefault(
                    friendRequest.Receiver.ProfilePictureUrl, "/images/avatars/user.png");
            }

            return friendRequests.ToList();
        }

        public async Task<List<FriendRequest>> GetReceivedFriendRequestAsync(int userId)
        {
            return (await _friendRequestRepository.FindAsync(
                f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending,
                noTracking: true,
                f => f.Sender,
                f => f.Receiver)).ToList();
        }

        public async Task<List<Friendship>> GetFriendsAsync(int userId)
        {
            return (await _friendshipRepository.FindAsync(
                f => f.SenderId == userId || f.ReceiverId == userId,
                noTracking: true,
                f => f.Sender,
                f => f.Receiver)).ToList();
        }
    }
}