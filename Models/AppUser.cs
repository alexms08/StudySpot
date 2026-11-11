using Microsoft.AspNetCore.Identity;
namespace StudySpot.Models;
public class AppUser : IdentityUser { public ICollection<Review> Reviews { get; set; } = new List<Review>(); }