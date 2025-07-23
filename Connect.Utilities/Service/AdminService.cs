using System.Linq.Expressions;
using System.Threading.Tasks;
using Connect.DataAccess.Data;
using Connect.DataAccess.Repository.IRepository;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace Connect.Utilities.Service
{
    public class AdminService : IAdminService
    {
        private readonly IGenericRepository<Post> _postRepository;
        private readonly IGenericRepository<Report> _reportRepository;

        public AdminService(
            IGenericRepository<Post> postRepository,
            IGenericRepository<Report> reportRepository)
        {
            _postRepository = postRepository;
            _reportRepository = reportRepository;
        }

        public async Task<List<Post>> GetReportedPostsAsync()
        {
            return (await _postRepository.FindAsync(
                p => p.NrOfReports >= 1,
                noTracking: true,
                p => p.User)).ToList();
        }

        public async Task ApproveReportAsync(int postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post != null)
            {
                await _postRepository.SoftDeleteAsync(post);
            }
        }

        public async Task RejectReportAsync(int postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post != null)
            {
                post.NrOfReports = 0;
                _postRepository.Update(post);
            }

            var postReports = await _reportRepository.FindAsync(r => r.PostId == postId);
            if (postReports.Any())
            {
                _reportRepository.RemoveRange(postReports);
            }
        }
    }
}