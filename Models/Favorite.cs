namespace StudySpot.Models;
public class Favorite { public int Id { get; set; } public int SpotId { get; set; } public Spot Spot { get; set; } = null!; public string UserId { get; set; } = ""; }