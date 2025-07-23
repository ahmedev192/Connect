using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Domain;
using Connect.Domain.DTOs;
namespace Connect.Application.Interfaces
{
    public interface IInteractionService
    {
        Task<NotificationDTO> TogglePostLikeAsync(int postId, int userId);
        Task AddCommentAsync(Comment comment, int userId);
        Task<int?> DeleteCommentAsync(int commentId, int userId);
        Task<NotificationDTO> TogglePostFavoriteAsync(int postId, int userId);

        Task AddPostReportAsync(int postId, int userId);
    }
}
