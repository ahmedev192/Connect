using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Connect.Application.Dtos;
using Connect.Application.Interfaces;
using Connect.Application.StaticDetails;
using Connect.Domain.Dtos;
using Connect.Domain.Entities;
using Connect.Infrastructure.Repository.IRepository;

namespace Connect.Application.Services
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

        public async Task<FriendRequest> UpdateRequestAsync(int requestId, FriendRequestStatus newStatus)
        {
            var request = await _unitOfWork.FriendRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new Exception("Friend request not found.");

            request.Status = newStatus;
            request.DateUpdated = DateTime.UtcNow;
            _unitOfWork.FriendRequestRepository.Update(request);

            if (newStatus == FriendRequestStatus.Accepted)
            {
                var friendship = new Friendship
                {
                    User1Id = Math.Min(request.SenderId, request.ReceiverId),
                    User2Id = Math.Max(request.SenderId, request.ReceiverId),
                    DateCreated = DateTime.UtcNow
                };
                await _unitOfWork.FriendshipRepository.AddAsync(friendship);
            }

            await _unitOfWork.SaveChangesAsync();
            return request;
        }

        public async Task SendRequestAsync(int senderId, int receiverId)
        {
            if (senderId == receiverId)
                throw new Exception("Cannot send friend request to self.");

            // Check for existing friendship
            var existingFriendship = await _unitOfWork.FriendshipRepository.FindAsync(
                f => (f.User1Id == Math.Min(senderId, receiverId) && f.User2Id == Math.Max(senderId, receiverId)),
                noTracking: true);

            if (existingFriendship.Any())
                throw new Exception("Users are already friends.");

            // Check for existing invitations
            var existingRequest = await _unitOfWork.FriendRequestRepository.FindAsync(
                r => (r.SenderId == senderId && r.ReceiverId == receiverId) && r.Status == FriendRequestStatus.Pending,
                noTracking: true);

            if (existingRequest.Any())
                throw new Exception("Friend request already sent.");

            var request = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = FriendRequestStatus.Pending,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            await _unitOfWork.FriendRequestRepository.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();
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
                        r => (r.SenderId == friendship.User1Id && r.ReceiverId == friendship.User2Id) ||
                             (r.SenderId == friendship.User2Id && r.ReceiverId == friendship.User1Id));

                    if (requests.Any())
                    {
                        _unitOfWork.FriendRequestRepository.RemoveRange(requests);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
            });
        }

        public async Task<List<UserWithFriendsCountDto>> GetSuggestedFriendsAsync(int userId)
        {
            var existingFriendIds = await _unitOfWork.FriendshipRepository.SelectAsync(
                f => f.User1Id == userId ? f.User2Id : f.User1Id,
                f => f.User1Id == userId || f.User2Id == userId,
                noTracking: true);

            var pendingRequestIds = await _unitOfWork.FriendRequestRepository.SelectAsync(
                r => r.SenderId == userId ? r.ReceiverId : r.SenderId,
                r => (r.SenderId == userId || r.ReceiverId == userId) && r.Status == FriendRequestStatus.Pending,
                noTracking: true);

            var suggestedFriends = await _unitOfWork.UserRepository.TakeAsync(
                count: 5,
                predicate: u => u.Id != userId && !existingFriendIds.Contains(u.Id) && !pendingRequestIds.Contains(u.Id),
                noTracking: true);


            foreach(var user in suggestedFriends)
            {
                user.ProfilePictureUrl = _fileUploadService.ResolveImageOrDefault(
                    user.ProfilePictureUrl, "/images/avatars/user.png");
            }


            var result = new List<UserWithFriendsCountDto>();
            foreach (var user in suggestedFriends)
            {
                var friendsCount = await _unitOfWork.FriendshipRepository.CountAsync(
                    f => f.User1Id == user.Id || f.User2Id == user.Id);
                result.Add(new UserWithFriendsCountDto
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
                f => f.SenderId == userId && f.Status == FriendRequestStatus.Pending,
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
            var friendRequests = await _unitOfWork.FriendRequestRepository.FindAsync(
                f => f.ReceiverId == userId && f.Status == FriendRequestStatus.Pending,
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

        public async Task<List<Friendship>> GetFriendsAsync(int userId)
        {
            var friendshipsAsUser1 = await _unitOfWork.FriendshipRepository.FindAsync(
                f => f.User1Id == userId,
                noTracking: true,
                f => f.User2);

            var friendshipsAsUser2 = await _unitOfWork.FriendshipRepository.FindAsync(
                f => f.User2Id == userId,
                noTracking: true,
                f => f.User1);

            return friendshipsAsUser1.Concat(friendshipsAsUser2).ToList();
        }

        public async Task CancelFriendRequestAsync(int requestId, int userId)
        {
            var request = await _unitOfWork.FriendRequestRepository.GetByIdAsync(requestId);
            if (request == null || request.SenderId != userId)
                throw new Exception("Invalid request or unauthorized action.");

            _unitOfWork.FriendRequestRepository.Remove(request);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}