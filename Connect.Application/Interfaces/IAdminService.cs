using Connect.Domain.Entities;

namespace Connect.Application.Interfaces
{
    public interface IAdminService
    {
        Task<List<Post>> GetReportedPostsAsync();
        Task ApproveReportAsync(int postId);
        Task RejectReportAsync(int postId);



    }
}   
