using ConnectApp.Data;
using Microsoft.EntityFrameworkCore;

namespace ConnectApp
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

            var app = builder.Build();


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
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();
                    var logger = services.GetRequiredService<ILogger<Program>>();

                    logger.LogInformation("Applying pending migrations...");
                    dbContext.Database.Migrate();

                    logger.LogInformation("Seeding database...");
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
