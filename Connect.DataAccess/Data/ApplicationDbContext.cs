using Connect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Connect.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Customize the ASP.NET identity model table names
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

            //Posts

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            //Likes

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

            //Comments

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



            //Favorites
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.PostId, f.UserId });

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Post)
                .WithMany(p => p.Favorites)
                .HasForeignKey(f => f.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Reports
            modelBuilder.Entity<Report>()
                .HasKey(f => new { f.PostId, f.UserId });

            modelBuilder.Entity<Report>()
                .HasOne(f => f.Post)
                .WithMany(p => p.Reports)
                .HasForeignKey(f => f.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Report>()
                .HasOne(f => f.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<User>()
       .HasMany(u => u.Stories)
       .WithOne(p => p.User)
       .HasForeignKey(p => p.UserId);

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
