using System.Linq.Expressions;
using System.Threading.Tasks;
using Connect.Infrastructure.Repository;
using Connect.Infrastructure.Repository.IRepository;
using Connect.Domain;
using Connect.Domain.DTOs;
using Connect.Application.Interfaces;
using Connect.Application.StaticDetails;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Service
{
    public class FriendService : IFriendService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;

        public FriendService(IUnitOfWork unitOfWork, IFileUploadService fileUploadService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
        }

        public async Task<FriendRequest> UpdateRequestAsync(int requestId, string newStatus)
        {
            var request = await _unitOfWork.FriendRequestRepository.GetByIdAsync(requestId);
            if (request != null)
            {
                request.Status = newStatus;
                request.DateUpdated = DateTime.UtcNow;
                _unitOfWork.FriendRequestRepository.Update(request);

                if (newStatus == FriendshipStatus.Accepted)
                {
                    var friendship = new Friendship
                    {
                        SenderId = request.SenderId,
                        ReceiverId = request.ReceiverId,
                        DateCreated = DateTime.UtcNow
                    };
                    await _unitOfWork.FriendshipRepository.AddAsync(friendship);
                }
                await _unitOfWork.SaveChangesAsync();
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
            await _unitOfWork.FriendRequestRepository.AddAsync(request);
        }

        public async Task RemoveFriendAsync(int friendshipId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var friendship = await _unitOfWork.FriendshipRepository.GetByIdAsync(friendshipId);
                if (friendship != null)
                {
                    _unitOfWork.FriendshipRepository.Remove(friendship);

                    var requests = await _unitOfWork.FriendRequestRepository.FindAsync(
                        r => (r.SenderId == friendship.SenderId && r.ReceiverId == friendship.ReceiverId) ||
                             (r.SenderId == friendship.ReceiverId && r.ReceiverId == friendship.SenderId));
                    if (requests.Any())
                    {
                        _unitOfWork.FriendRequestRepository.RemoveRange(requests);
                    }
                }
            });
        }

        public async Task<List<UserWithFriendsCountDTO>> GetSuggestedFriendsAsync(int userId)
        {
            var existingFriendIds = await _unitOfWork.FriendshipRepository.SelectAsync(
                f => f.SenderId == userId ? f.ReceiverId : f.SenderId,
                f => f.SenderId == userId || f.ReceiverId == userId,
                noTracking: true);

            var pendingRequestIds = await _unitOfWork.FriendRequestRepository.SelectAsync(
                r => r.SenderId == userId ? r.ReceiverId : r.SenderId,
                r => (r.SenderId == userId || r.ReceiverId == userId) && r.Status == FriendshipStatus.Pending,
                noTracking: true);

            var suggestedFriends = await _unitOfWork.UserRepository.TakeAsync(
                count: 5,
                predicate: u => u.Id != userId && !existingFriendIds.Contains(u.Id) && !pendingRequestIds.Contains(u.Id),
                noTracking: true);

            var result = new List<UserWithFriendsCountDTO>();
            foreach (var user in suggestedFriends)
            {
                var friendsCount = await _unitOfWork.FriendshipRepository.CountAsync(f => f.SenderId == user.Id || f.ReceiverId == user.Id);
                result.Add(new UserWithFriendsCountDTO
                {
                    User = user,
                    FriendsCount = friendsCount
                });
            }

            return result;
        }

        public async Task<List<FriendRequest>> GetSentFriendRequestAsync(int userId)
        {
            var friendRequests = await _unitOfWork.FriendRequestRepository.FindAsync(
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
            return (await _unitOfWork.FriendRequestRepository.FindAsync(
                f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending,
                noTracking: true,
                f => f.Sender,
                f => f.Receiver)).ToList();
        }

        public async Task<List<Friendship>> GetFriendsAsync(int userId)
        {
            return (await _unitOfWork.FriendshipRepository.FindAsync(
                f => f.SenderId == userId || f.ReceiverId == userId,
                noTracking: true,
                f => f.Sender,
                f => f.Receiver)).ToList();
        }
    }
}