using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudySpot.Models;
namespace StudySpot.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options) {
 public DbSet<Spot> Spots => Set<Spot>(); public DbSet<Review> Reviews => Set<Review>(); public DbSet<Favorite> Favorites => Set<Favorite>();
 protected override void OnModelCreating(ModelBuilder b) { base.OnModelCreating(b); b.Entity<Favorite>().HasIndex(x=>new{x.UserId,x.SpotId}).IsUnique(); b.Entity<Review>().HasIndex(x=>new{x.UserId,x.SpotId}).IsUnique(); }
}