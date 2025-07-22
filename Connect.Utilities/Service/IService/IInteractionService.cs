using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Models;

namespace Connect.Utilities.Service.IService
{
    public interface IInteractionService
    {
        Task TogglePostLikeAsync(int postId, int userId);
        Task AddCommentAsync(Comment comment, int userId);
        Task<int?> DeleteCommentAsync(int commentId, int userId);
        Task TogglePostFavoriteAsync(int postId, int userId);
        Task AddPostReportAsync(int postId, int userId);
    }
}
