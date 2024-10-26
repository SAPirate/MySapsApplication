using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySapsApplication.Models.Suspects;
using MySapsApplication.Models.ViewModel;
using System.Threading.Tasks;
using System;

namespace MySapsApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddMvc().AddControllersAsServices();

            // Database Context Configuration
            var connectionString = "Server=LAPTOP-60Q8F6JH\\SQLEXPRESS;Database=MySapsApplication;Trusted_Connection=True;TrustServerCertificate=True;";
            builder.Services.AddDbContext<SuspectsDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
           

            // Identity Configuration
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Configure Identity Options
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true; // Set your password policy
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Home}/{id?}");

            // Role Initialization
            await InitializeRolesAndUsers(app.Services);

            app.Run();
        }

        private static async Task InitializeRolesAndUsers(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var roles = new[] { "Admin", "Manager", "Member","CaseAdmin" };

            // Create roles
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create users
            await CreateUserIfNotExists(userManager, "manager@manager.com", "Manager123,", "Manager");
            await CreateUserIfNotExists(userManager, "casemanager@1.com", "Sam123,", "Admin");
            await CreateUserIfNotExists(userManager, "casemanager@2.com", "Joe123,", "Member");
            await CreateUserIfNotExists(userManager, "case@admin.com", "Admin123,", "CaseAdmin");
        }

        private static async Task CreateUserIfNotExists(UserManager<IdentityUser> userManager, string email, string password, string role)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    // Handle errors (log them or throw an exception)
                }
            }
        }
    }
}
