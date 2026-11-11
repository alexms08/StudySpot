using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudySpot.Data;
using StudySpot.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(o => o.HeaderName = "X-CSRF-TOKEN");
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddIdentity<AppUser, IdentityRole>(o => { o.SignIn.RequireConfirmedAccount = false; o.Password.RequiredLength = 6; o.Password.RequireNonAlphanumeric = false; o.Password.RequireUppercase = true; o.Lockout.MaxFailedAccessAttempts = 5; o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(o => { o.LoginPath = "/Account/Login"; o.AccessDeniedPath = "/Account/Login"; });
var app = builder.Build();
using (var scope = app.Services.CreateScope()) await Seed.Initialize(scope.ServiceProvider);
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseStatusCodePagesWithReExecute("/Home/Status", "?code={0}"); app.UseStaticFiles(); app.UseRouting(); app.UseAuthentication(); app.UseAuthorization();
app.MapControllerRoute(name:"default",pattern:"{controller=Home}/{action=Index}/{id?}");
app.Run();
