using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySpot.Data;
using StudySpot.Models;
namespace StudySpot.Controllers;
[Authorize(Roles="Admin")] public class AdminController(AppDbContext db):Controller {
 public async Task<IActionResult> Index()=>View(await db.Spots.OrderBy(x=>x.Name).ToListAsync());
 public IActionResult Create()=>View("Edit",new Spot());
 public async Task<IActionResult> Edit(int id){var s=await db.Spots.FindAsync(id);return s==null?NotFound():View(s);}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Save([Bind("Id,Name,Address,City,Category,Description,Latitude,Longitude,PricePerHour,Wifi,Outlets,Quiet,OpeningHours")] Spot spot){
  ModelState.Remove(nameof(Spot.Reviews));
  if(!ModelState.IsValid)return View("Edit",spot);
  if(spot.Id==0)db.Spots.Add(spot);else{var original=await db.Spots.FindAsync(spot.Id);if(original==null)return NotFound();db.Entry(original).CurrentValues.SetValues(spot);}
  await db.SaveChangesAsync();return RedirectToAction(nameof(Index));
 }
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Delete(int id){var s=await db.Spots.FindAsync(id);if(s!=null){db.Spots.Remove(s);await db.SaveChangesAsync();}return RedirectToAction(nameof(Index));}
}