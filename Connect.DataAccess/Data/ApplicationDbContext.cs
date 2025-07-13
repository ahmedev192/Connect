using Connect.Models;
using Microsoft.EntityFrameworkCore;

namespace Connect.DataAccess.Data
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







            //modelBuilder.Entity<User>().HasData(
            //new User
            //{
            //    Id = 1,
            //    FullName = "Ahmed Mahmoud",
            //    ProfilePictureUrl = " "
            //},
            //new User
            //{
            //    Id = 2,
            //    FullName = "Youssef Mostafa",
            //    ProfilePictureUrl = " "
            //});



            //modelBuilder.Entity<Post>().HasData(
            //    new Post
            //    {
            //        Id = 1,
            //        Content = "Welcome to Connect! This is your first post.",
            //        ImageUrl = null,
            //        NrOfReports = 0,
            //        DateCreated = new DateTime(2025, 7, 12, 14, 34, 0, DateTimeKind.Utc),
            //        DateUpdated = new DateTime(2025, 7, 12, 14, 34, 0, DateTimeKind.Utc),
            //        UserId = 1
            //    },
            //    new Post
            //    {
            //        Id = 2,
            //        Content = "Connect is designed to help you connect with others.",
            //        ImageUrl = null,
            //        NrOfReports = 0,
            //        DateCreated = new DateTime(2025, 7, 12, 14, 34, 0, DateTimeKind.Utc),
            //        DateUpdated = new DateTime(2025, 7, 12, 14, 34, 0, DateTimeKind.Utc),
            //        UserId = 2

            //    },
            //    new Post
            //    {
            //        Id = 3,
            //        Content = "Feel free to explore and share your thoughts!",
            //        ImageUrl = null,
            //        NrOfReports = 0,
            //        DateCreated = new DateTime(2025, 7, 12, 14, 34, 0, DateTimeKind.Utc),
            //        DateUpdated = new DateTime(2025, 7, 12, 14, 34, 0, DateTimeKind.Utc),
            //        UserId = 1


            //    });





        }

    }
}
