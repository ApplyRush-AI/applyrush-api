using Application.Common.Interfaces;
using Application.Features.Languages.Commands;
using Domain.Entities.Medias;
using Domain.Entities.User;
using DTO.Enums.Media;
using DTO.Enums.User;
using MassTransit.Testing;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Persistence;

public static class ApplicationDbContextSeeder
{
    public static async Task<ApplicationUser> SeedDefaultRolesAndUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        var administratorRole = new IdentityRole<int>(UserRole.Administrator);
        var customerRole = new IdentityRole<int>(UserRole.Customer);

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name)) await roleManager.CreateAsync(administratorRole);
        if (roleManager.Roles.All(r => r.Name != customerRole.Name)) await roleManager.CreateAsync(customerRole);

        var administrator = new ApplicationUser
        {
            FirstName = "John",
            LastName = "Doe",
            UserName = "administrator@applyrush.com",
            Email = "administrator@applyrush.com",
            EmailConfirmed = true,
            Media = new Media(MediaEntityType.User),
            Status = UserStatus.Active
        };

        // Check if default administrator already created
        var existedAdministrator = userManager.Users.FirstOrDefault(u => u.UserName == administrator.UserName);
        if (existedAdministrator != null) return existedAdministrator;

        // Create default administraotor
        await userManager.CreateAsync(administrator, "Administrator1!");
        await userManager.AddClaimAsync(administrator, new Claim("scope", "default"));

        // Add the user to the role (Administrator)
        await userManager.AddToRolesAsync(
            administrator,
            new[] { administratorRole.Name! });

        return administrator;
    }

    public static async Task SeedDefaultLanguages(
        ISender mediatr,
        IApplicationDbContext dbContext)
    {
        if (!await dbContext.Language.AnyAsync())
        {
            await mediatr.Send(new LanguageCreateCommand("English", "en", "en-US", true));
        }
    }
}

