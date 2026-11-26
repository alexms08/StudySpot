using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudySpot.Models;

namespace StudySpot.Data;

public static class Seed
{
    public static async Task Initialize(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        // For a fresh prototype database only. Do not mix EnsureCreated with migrations.
        await db.Database.EnsureCreatedAsync();

        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roles.RoleExistsAsync("Admin"))
            await roles.CreateAsync(new IdentityRole("Admin"));

        // No fictitious places are seeded in the public directory.
        // Optional initial admin: create a NEW account from private environment variables.
        var settings = services.GetRequiredService<IConfiguration>();
        var email = settings["ADMIN_BOOTSTRAP_EMAIL"];
        var password = settings["ADMIN_BOOTSTRAP_PASSWORD"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        var users = services.GetRequiredService<UserManager<AppUser>>();
        var existing = await users.FindByEmailAsync(email);
        if (existing != null) return; // Never elevate an existing public account.
        var admin = new AppUser { UserName = email, Email = email, EmailConfirmed = true };
        var created = await users.CreateAsync(admin, password);
        if (created.Succeeded) await users.AddToRoleAsync(admin, "Admin");
        else throw new InvalidOperationException("Admin bootstrap credentials are invalid.");
    }
}
