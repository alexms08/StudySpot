using System.ComponentModel.DataAnnotations;
namespace StudySpot.Models;
public class Spot {
 public int Id { get; set; }
 [Required,StringLength(100)] public string Name { get; set; } = "";
 [Required,StringLength(150)] public string Address { get; set; } = "";
 [Required,StringLength(50)] public string City { get; set; } = "";
 [Required,StringLength(40)] public string Category { get; set; } = "Библиотека";
 [StringLength(600)] public string Description { get; set; } = "";
 [Range(-90,90)] public double Latitude { get; set; }
 [Range(-180,180)] public double Longitude { get; set; }
 [Range(0,100)] public int PricePerHour { get; set; }
 public bool Wifi { get; set; } public bool Outlets { get; set; } public bool Quiet { get; set; }
 [StringLength(100)] public string OpeningHours { get; set; } = "";
 public ICollection<Review> Reviews { get; set; } = new List<Review>();
}