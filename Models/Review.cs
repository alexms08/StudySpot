using System.ComponentModel.DataAnnotations;
namespace StudySpot.Models;
public class Review {
 public int Id { get; set; } public int SpotId { get; set; } public Spot Spot { get; set; } = null!;
 public string UserId { get; set; } = ""; public AppUser User { get; set; } = null!;
 [Range(1,5)] public int Rating { get; set; }
 [Required,StringLength(500)] public string Comment { get; set; } = "";
 public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}