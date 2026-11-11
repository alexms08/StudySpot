using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySpot.Data;
using StudySpot.Models;
namespace StudySpot.Controllers;
[Authorize] public class FavoritesController(AppDbContext db):Controller {
 public async Task<IActionResult> Index(){var uid=User.FindFirstValue(ClaimTypes.NameIdentifier);return View(await db.Favorites.Include(x=>x.Spot).Where(x=>x.UserId==uid).ToListAsync());}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Toggle(int spotId){
  if(!await db.Spots.AnyAsync(x=>x.Id==spotId))return NotFound();var uid=User.FindFirstValue(ClaimTypes.NameIdentifier)!;
  var f=await db.Favorites.FirstOrDefaultAsync(x=>x.UserId==uid&&x.SpotId==spotId);if(f==null)db.Favorites.Add(new Favorite{SpotId=spotId,UserId=uid});else db.Favorites.Remove(f);
  await db.SaveChangesAsync();return RedirectToAction("Details","Home",new{id=spotId});
 }
}