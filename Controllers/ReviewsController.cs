using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySpot.Data;
using StudySpot.Models;
namespace StudySpot.Controllers;
[Authorize] public class ReviewsController(AppDbContext db):Controller {
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Add(int spotId,int rating,string comment) {
  if(!await db.Spots.AnyAsync(s=>s.Id==spotId))return NotFound();
  if(rating<1||rating>5||string.IsNullOrWhiteSpace(comment)||comment.Length>500){TempData["Error"]="Въведете оценка от 1 до 5 и коментар до 500 символа.";return RedirectToAction("Details","Home",new{id=spotId});}
  comment=comment.Trim();
  var uid=User.FindFirstValue(ClaimTypes.NameIdentifier)!;
  var existing=await db.Reviews.FirstOrDefaultAsync(r=>r.UserId==uid&&r.SpotId==spotId);
  if(existing==null)db.Reviews.Add(new Review{SpotId=spotId,UserId=uid,Rating=rating,Comment=comment});else{existing.Rating=rating;existing.Comment=comment;existing.CreatedAt=DateTime.UtcNow;}
  await db.SaveChangesAsync();return RedirectToAction("Details","Home",new{id=spotId});
 }
}