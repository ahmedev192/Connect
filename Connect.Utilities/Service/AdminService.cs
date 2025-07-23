using System.Linq.Expressions;
using System.Threading.Tasks;
using Connect.DataAccess.Repository;
using Connect.DataAccess.Repository.IRepository;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace Connect.Utilities.Service
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Post>> GetReportedPostsAsync()
        {
            return (await _unitOfWork.PostRepository.FindAsync(
                p => p.NrOfReports >= 1,
                noTracking: true,
                p => p.User)).ToList();
        }

        public async Task ApproveReportAsync(int postId)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(postId);
            if (post != null)
            {
                await _unitOfWork.PostRepository.SoftDeleteAsync(post);
            }
        }

        public async Task RejectReportAsync(int postId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var post = await _unitOfWork.PostRepository.GetByIdAsync(postId);
                if (post != null)
                {
                    post.NrOfReports = 0;
                    _unitOfWork.PostRepository.Update(post);
                }

                var postReports = await _unitOfWork.ReportRepository.FindAsync(r => r.PostId == postId);
                if (postReports.Any())
                {
                    _unitOfWork.ReportRepository.RemoveRange(postReports);
                }
            });
        }
    }
}