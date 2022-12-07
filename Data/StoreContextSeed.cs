using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using dotnet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace dotnet.Data
{
    public class StoreContextSeed
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<ApplicationDbContext>();

        string[] roles = new string[] { "Owner", "Administrator", "Manager", "Editor", "Buyer", "Business", "Seller", "Subscriber" };

        foreach (string role in roles)
        {
            var roleStore = new RoleStore<IdentityRole>(context);

            if (!context.Roles.Any(r => r.Name == role))
            {
                await roleStore.CreateAsync(new IdentityRole(role));
            }
        }

        var stockData= File.ReadAllText("Data/StocksSeedNew.json");

        var stock= JsonSerializer.Deserialize<List<Stock>>(stockData);

        foreach (var item in stock)
        {
            context.Stocks.Add(item);
        }


        var user = new IdentityUser
        {
            Email = "test@test.com",
            NormalizedEmail = "TEST@TEST.COM",
            UserName = "Owner",
            NormalizedUserName = "OWNER",
            PhoneNumber = "+111111111111",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D")
        };


        if (!context.Users.Any(u => u.UserName == user.UserName))
        {
            var password = new PasswordHasher<IdentityUser>();
            var hashed = password.HashPassword(user,"secret");
            user.PasswordHash = hashed;

            var userStore = new UserStore<IdentityUser>(context);
            var result = await userStore.CreateAsync(user);

        }

        await AssignRoles(serviceProvider, user.Email, roles);



        await context.SaveChangesAsync();
    }

    public static async Task<IdentityResult> AssignRoles(IServiceProvider services, string email, string[] roles)
    {
        UserManager<IdentityUser> _userManager = services.GetService<UserManager<IdentityUser>>();
        IdentityUser user = await _userManager.FindByEmailAsync(email);
        var result = await _userManager.AddToRolesAsync(user, roles);

        return result;
    }
}
}