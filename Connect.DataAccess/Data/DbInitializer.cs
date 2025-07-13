using System;
using Connect.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Connect.DataAccess.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext applicationDbContext, ILogger logger)
        {
            try
            {
                var hasUsers = applicationDbContext.Users.Any();
                var hasPosts = applicationDbContext.Posts.Any();

                if (!hasUsers && !hasPosts)
                {
                    var users = new List<User>
            {
                new User { FullName = "Ahmed Mahmoud", ProfilePictureUrl = "https://example.com/profile1.jpg" },
                new User { FullName = "Youssef Mostafa", ProfilePictureUrl = "https://example.com/profile2.jpg" }
            };

                    await applicationDbContext.Users.AddRangeAsync(users);
                    await applicationDbContext.SaveChangesAsync(); // Save so IDs are generated

                    // Retrieve actual IDs after save
                    var ahmed = users.FirstOrDefault(u => u.FullName == "Ahmed Mahmoud");
                    var youssef = users.FirstOrDefault(u => u.FullName == "Youssef Mostafa");

                    var posts = new List<Post>
            {
                new Post
                {
                    Content = "Welcome to Connect! This is your first post.",
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    NrOfReports = 0,
                    UserId = ahmed!.Id
                },
                new Post
                {
                    Content = "Connect is designed to help you connect with others.",
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    NrOfReports = 0,
                    UserId = youssef!.Id
                },
                new Post
                {
                    Content = "Feel free to explore and share your thoughts!",
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    NrOfReports = 0,
                    UserId = ahmed!.Id
                }
            };

                    await applicationDbContext.Posts.AddRangeAsync(posts);
                    await applicationDbContext.SaveChangesAsync();

                    logger.LogInformation("Database seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Database already contains users and posts. Seeding skipped.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during database seeding.");
            }
        }

    }
}
