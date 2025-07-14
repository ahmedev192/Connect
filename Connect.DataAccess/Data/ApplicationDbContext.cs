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
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade); // or Restrict if you want manual cleanup

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);



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
