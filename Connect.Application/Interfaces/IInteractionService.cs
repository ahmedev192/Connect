using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Domain.Entities;
using Connect.Domain.Dtos;
using Connect.Domain.Entities;
namespace Connect.Application.Interfaces
{
    public interface IInteractionService
    {
        Task<NotificationDto> TogglePostLikeAsync(int postId, int userId);
        Task AddCommentAsync(Comment comment, int userId);
        Task<int?> DeleteCommentAsync(int commentId, int userId);
        Task<NotificationDto> TogglePostFavoriteAsync(int postId, int userId);

        Task AddPostReportAsync(int postId, int userId);
    }
}
