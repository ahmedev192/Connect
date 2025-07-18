using Connect.DataAccess.Data;
using Connect.DataAccess.Repository;
using Connect.DataAccess.Repository.IRepository;
using Connect.Models;
using Connect.Utilities.Service;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace Connect
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");


            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));
            builder.Services.AddScoped<IFileUploadService, FileUploadService>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IHashtagService, HashtagService>();
            builder.Services.AddScoped<IUsersService, UsersService>();





            builder.Services.AddRazorPages();
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            builder.Services
                .AddIdentity<User, IdentityRole<int>>(options =>
                {
                    // SignIn settings
                    options.SignIn.RequireConfirmedAccount = false;

                    // Password settings
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    //options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 4;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Authentication/Login";
                options.AccessDeniedPath = "/Authentication/AccessDenied";
            });
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();




            var app = builder.Build();

            app.UseAuthentication();
            app.MapRazorPages();

            //bad code,  it's not supporting the migrations and not compatable with EF

            //using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //    // Ensure the database is created and seed initial data
            //    dbContext.Database.EnsureCreated();
            //    DbInitializer.SeedAsync(dbContext).GetAwaiter().GetResult();
            //}



            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
                    await DbInitializer.SeedUsersAndRolesAsync(userManager, roleManager);

                    var dbContext = services.GetRequiredService<ApplicationDbContext>();
                    var logger = services.GetRequiredService<ILogger<Program>>();

                    await dbContext.Database.MigrateAsync();

                    await DbInitializer.SeedAsync(dbContext, logger);



                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                }
            }


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
