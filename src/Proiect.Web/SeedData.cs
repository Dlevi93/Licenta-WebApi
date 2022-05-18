using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Proiect.Infrastructure.Data;
using Proiect.Infrastructure.Data.Auth;

namespace Proiect.Web
{
    public static class SeedData
    {
        public static void InitializeData(IServiceProvider serviceProvider)
        {
            using var dbContext = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

            PopulateAdmin(dbContext, serviceProvider);
            PopulateUser1(dbContext, serviceProvider);
        }

        public static void PopulateAdmin(AppDbContext context, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (context.Users.Any(x => x.UserName == "admin")) return;

            var user = new ApplicationUser
            {
                Email = "admin@licenta.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "admin",
                FullName = "Levi Deak",
            };

            if (context.Roles.Count(x => x.Name == "Admin") == 0)
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };
                var roleManagerResult = roleManager.CreateAsync(role).Result;
            }

            var res = userManager.CreateAsync(user, "Admin@123").Result;
            var roleRes = userManager.AddToRoleAsync(user, "Admin").Result;
        }

        public static void PopulateUser1(AppDbContext context, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (context.Users.Any(x => x.UserName == "user1")) return;

            var user = new ApplicationUser
            {
                Email = "user1@medii.ro",
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "user1",
                FullName = "User 1",
            };

            if (context.Roles.Count(x => x.Name == "Admin") == 0)
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };
                var roleManagerResult = roleManager.CreateAsync(role).Result;
            }

            var res = userManager.CreateAsync(user, "User1@123").Result;
            var roleRes = userManager.AddToRoleAsync(user, "Admin").Result;
        }
    }
}
