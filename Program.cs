using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StudySpot.Data;
using StudySpot.Models;

var builder = WebApplication.CreateBuilder(args);
// Diploma application: listen only on the local computer.
builder.WebHost.ConfigureKestrel(o => o.ListenLocalhost(5080));
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RENDER")))
    throw new InvalidOperationException("Public hosting is disabled for this diploma project.");
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(o => o.HeaderName = "X-CSRF-TOKEN");

var dbUrl = builder.Configuration["DATABASE_URL"];
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(dbUrl))
    {
        if (dbUrl.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
            dbUrl.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(dbUrl);
            var credentials = uri.UserInfo.Split(':', 2);
            if (credentials.Length != 2) throw new InvalidOperationException("Invalid PostgreSQL connection URL");
            var connection = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host, Port = uri.Port > 0 ? uri.Port : 5432,
                Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
                Username = Uri.UnescapeDataString(credentials[0]),
                Password = Uri.UnescapeDataString(credentials[1]),
                SslMode = SslMode.Require
            };
            options.UseNpgsql(connection.ConnectionString);
        }
        else options.UseNpgsql(dbUrl);
    }
    else
        options.UseSqlite(builder.Configuration.GetConnectionString("Default"));

});

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false; // Requires email delivery before enabling.
    options.Password.RequiredLength = 10;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Account/Login";
    o.AccessDeniedPath = "/Account/Login";
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.Lax;
    o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("auth", limit =>
    {
        limit.Window = TimeSpan.FromMinutes(5);
        limit.PermitLimit = 12;
        limit.QueueLimit = 0;
    });
    options.AddFixedWindowLimiter("submissions", limit =>
    {
        limit.Window = TimeSpan.FromMinutes(10);
        limit.PermitLimit = 20;
        limit.QueueLimit = 0;
    });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
    await Seed.Initialize(scope.ServiceProvider);
if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Home/Error");

app.UseStatusCodePagesWithReExecute("/Home/Status", "?code={0}");
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", async (AppDbContext db) =>
    await db.Database.CanConnectAsync() ? Results.Ok("healthy") : Results.StatusCode(503));
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

public partial class Program { }
