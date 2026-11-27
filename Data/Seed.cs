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

        // Four sourced public library records; amenities and hours are unknown.
        if (!await db.Spots.AnyAsync())
        {
            db.Spots.AddRange(
                new Spot { Name = "Регионална библиотека „Пейо К. Яворов“", City = "Бургас",
                    Address = "ул. „Св. св. Кирил и Методий“ № 43", Category = "Библиотека",
                    Description = "Обществена библиотека и културно пространство. Проверете условията за ползване на читалните преди посещение.",
                    Website = "https://burgaslib.bg/" },
                new Spot { Name = "Регионална библиотека „Пенчо Славейков“", City = "Варна",
                    Address = "бул. „Сливница“ № 34", Category = "Библиотека",
                    Description = "Обществена библиотека, която обслужва и студенти. Проверете достъпа и работното време на официалния сайт.",
                    Website = "https://www.libvar.bg/" },
                new Spot { Name = "Столична библиотека", City = "София",
                    Address = "пл. „Славейков“ № 4", Category = "Библиотека",
                    Description = "Обществена библиотека в центъра на София. Условията за читалните може да се променят.",
                    Website = "https://www.libsofia.bg/" },
                new Spot { Name = "Народна библиотека „Иван Вазов“", City = "Пловдив",
                    Address = "ул. „Авксентий Велешки“ № 17", Category = "Библиотека",
                    Description = "Обществена библиотека с читални и библиотечни услуги. Проверете актуалните правила за достъп.",
                    Website = "https://www.libplovdiv.com/" });
            await db.SaveChangesAsync();
        }

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
