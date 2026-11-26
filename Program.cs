using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudySpot.Data;
using StudySpot.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(o => o.HeaderName = "X-CSRF-TOKEN");
var postgres = builder.Configuration["DATABASE_URL"];
builder.Services.AddDbContext<AppDbContext>(o => {
 if (!string.IsNullOrWhiteSpace(postgres)) {
  var uri = new Uri(postgres);
  var credentials = uri.UserInfo.Split(':', 2);
  var cs = new Npgsql.NpgsqlConnectionStringBuilder { Host = uri.Host, Port = uri.Port > 0 ? uri.Port : 5432, Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')), Username = Uri.UnescapeDataString(credentials[0]), Password = Uri.UnescapeDataString(credentials[1]), SslMode = Npgsql.SslMode.Require };
  o.UseNpgsql(cs.ConnectionString);
 } else if (builder.Environment.IsDevelopment()) o.UseSqlite(builder.Configuration.GetConnectionString("Default"));
 else throw new InvalidOperationException("DATABASE_URL is required in production.");
});
builder.Services.AddIdentity<AppUser, IdentityRole>(o => { o.SignIn.RequireConfirmedAccount = false; o.Password.RequiredLength = 6; o.Password.RequireNonAlphanumeric = false; o.Password.RequireUppercase = true; o.Lockout.MaxFailedAccessAttempts = 5; o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(o => { o.LoginPath = "/Account/Login"; o.AccessDeniedPath = "/Account/Login"; });
var app = builder.Build();
using (var scope = app.Services.CreateScope()) await Seed.Initialize(scope.ServiceProvider);
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
if (app.Environment.IsDevelopment()) app.UseHttpsRedirection(); app.UseStatusCodePagesWithReExecute("/Home/Status", "?code={0}"); app.UseStaticFiles(); app.UseRouting(); app.UseAuthentication(); app.UseAuthorization();
app.MapControllerRoute(name:"default",pattern:"{controller=Home}/{action=Index}/{id?}");
app.MapGet("/health", async (AppDbContext db) => await db.Database.CanConnectAsync() ? Results.Ok("healthy") : Results.StatusCode(503));
app.Run();
