using ConnectApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>().HasData(
                new Post
                {
                    Id = 1,
                    Content = "Welcome to ConnectApp! This is your first post.",
                    ImageUrl = null,
                    NrOfReports = 0,
                    DateCreated = new DateTime(2025, 7, 12, 14, 34, 0, DateTimeKind.Utc),
                    DateUpdated = new DateTime(2025, 7, 12, 14, 34, 0, DateTimeKind.Utc)
                },
                new Post
                {
                    Id = 2,
                    Content = "ConnectApp is designed to help you connect with others.",
                    ImageUrl = null,
                    NrOfReports = 0,
                    DateCreated = new DateTime(2025, 7, 12, 14, 34, 0, DateTimeKind.Utc), 
                    DateUpdated = new DateTime(2025, 7, 12, 14, 34, 0, DateTimeKind.Utc)
                },
                new Post
                {
                    Id = 3,
                    Content = "Feel free to explore and share your thoughts!",
                    ImageUrl = null,
                    NrOfReports = 0,
                    DateCreated = new DateTime(2025, 7, 12, 14, 34, 0, DateTimeKind.Utc),
                    DateUpdated = new DateTime(2025, 7, 12, 14, 34, 0, DateTimeKind.Utc)

                });


            modelBuilder.Entity<User>().HasData(new User
            {
                  Id = 1,
                  FullName = "Ahmed Mahmoud",
                  ProfilePictureUrl = " "
            },
            new User
            {
                Id = 2,
                FullName = "Youssef Mostafa",
                ProfilePictureUrl = " "
            }
            );

               
        }

    }
}
