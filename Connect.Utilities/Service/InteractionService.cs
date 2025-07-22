using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.DataAccess.Data;
using Connect.Models;
using Connect.Utilities.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace Connect.Utilities.Service
{
    public class InteractionService : IInteractionService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;



        public InteractionService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task TogglePostLikeAsync(int postId, int userId)
        {
            var post = await _context.Posts
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                throw new Exception("Post not found");

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);

            if (existingLike != null)
                _context.Likes.Remove(existingLike);
            else
                await _context.Likes.AddAsync(new Like { PostId = postId, UserId = userId });

            await _context.SaveChangesAsync();
            //add notification to db
            await _notificationService.AddNewNotificationAsync(userId, "Someone liked your post", "Like");

        }

        public async Task AddCommentAsync(Comment comment, int userId)
        {
            comment.UserId = userId;
            comment.DateCreated = DateTime.UtcNow;
            comment.DateUpdated = DateTime.UtcNow;
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<int?> DeleteCommentAsync(int commentId, int userId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || comment.UserId != userId)
                return null;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return comment.PostId;
        }

        public async Task TogglePostFavoriteAsync(int postId, int userId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (favorite != null)
                _context.Favorites.Remove(favorite);
            else
                await _context.Favorites.AddAsync(new Favorite { PostId = postId, UserId = userId });

            await _context.SaveChangesAsync();
        }

        public async Task AddPostReportAsync(int postId, int userId)
        {
            var existingReport = await _context.Reports
                .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);

            if (existingReport != null)
                throw new Exception("You have already reported this post.");

            var report = new Report
            {
                PostId = postId,
                UserId = userId,
                DateCreated = DateTime.UtcNow
            };
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();
        }
    }
}
