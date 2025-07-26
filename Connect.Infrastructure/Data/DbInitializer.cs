using System;
using Connect.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Connect.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext db, ILogger logger)
        {
            if (await db.Users.AnyAsync()) return;

            var users = new List<User>
            {
                new User { UserName = "alice", Email = "alice@example.com", FullName = "Alice Johnson", EmailConfirmed = true, DateCreated = DateTime.UtcNow },
                new User { UserName = "bob", Email = "bob@example.com", FullName = "Bob Smith", EmailConfirmed = true, DateCreated = DateTime.UtcNow },
                new User { UserName = "carol", Email = "carol@example.com", FullName = "Carol Davis", EmailConfirmed = true, DateCreated = DateTime.UtcNow },
            };

            db.Users.AddRange(users);
            await db.SaveChangesAsync();

            var posts = new List<Post>
            {
                new Post { Content = "First post by Alice", UserId = users[0].Id, DateCreated = DateTime.UtcNow },
                new Post { Content = "Bob's travel thoughts", UserId = users[1].Id, DateCreated = DateTime.UtcNow },
                new Post { Content = "Carol shares an update", UserId = users[2].Id, DateCreated = DateTime.UtcNow }
            };

            db.Posts.AddRange(posts);
            await db.SaveChangesAsync();

            var comments = new List<Comment>
            {
                new Comment { Content = "Nice post!", UserId = users[1].Id, PostId = posts[0].Id, DateCreated = DateTime.UtcNow },
                new Comment { Content = "Thanks for sharing!", UserId = users[2].Id, PostId = posts[0].Id, DateCreated = DateTime.UtcNow },
                new Comment { Content = "I agree!", UserId = users[0].Id, PostId = posts[1].Id, DateCreated = DateTime.UtcNow }
            };

            db.Comments.AddRange(comments);
            await db.SaveChangesAsync();

            var likes = new List<Like>
            {
                new Like { UserId = users[1].Id, PostId = posts[0].Id, DateCreated = DateTime.UtcNow },
                new Like { UserId = users[2].Id, PostId = posts[0].Id, DateCreated = DateTime.UtcNow },
                new Like { UserId = users[0].Id, PostId = posts[2].Id, DateCreated = DateTime.UtcNow }
            };

            db.Likes.AddRange(likes);
            await db.SaveChangesAsync();

            logger.LogInformation("Dummy users, posts, comments, and likes seeded successfully.");
        }

        public static async Task SeedUsersAndRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            // Seed roles
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }

            // Seed default admin user
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "Ahmed"
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin@123");

                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create admin user: " +
                        string.Join("; ", result.Errors.Select(e => e.Description)));
                }

                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
    }
}
