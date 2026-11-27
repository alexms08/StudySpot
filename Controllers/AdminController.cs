using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySpot.Data;
using StudySpot.Models;

namespace StudySpot.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Suggestions = await db.PlaceSuggestions.AsNoTracking()
            .Where(s => s.Status == "Pending").OrderBy(s => s.CreatedAt).ToListAsync();
        return View(await db.Spots.AsNoTracking().OrderBy(x => x.Name).ToListAsync());
    }

    [HttpGet] public IActionResult Create() => View("Edit", new Spot());

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var spot = await db.Spots.FindAsync(id);
        return spot == null ? NotFound() : View(spot);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save([Bind("Id,Name,Address,City,Category,Description,Latitude,Longitude,PricePerHour,Wifi,Outlets,Quiet,OpeningHours,Website,IsVerified")] Spot spot)
    {
        ModelState.Remove(nameof(Spot.Reviews));
        ModelState.Remove(nameof(Spot.Favorites));
        if (!ModelState.IsValid) return View("Edit", spot);
        if (spot.Id == 0) db.Spots.Add(spot);
        else
        {
            var existing = await db.Spots.FindAsync(spot.Id);
            if (existing == null) return NotFound();
            db.Entry(existing).CurrentValues.SetValues(spot);
        }
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var spot = await db.Spots.FindAsync(id);
        if (spot != null) { db.Spots.Remove(spot); await db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ReviewSuggestion(int id, bool approve)
    {
        var suggestion = await db.PlaceSuggestions.FindAsync(id);
        if (suggestion == null) return NotFound();
        if (suggestion.Status != "Pending") return RedirectToAction(nameof(Index));
        if (approve)
        {
            db.Spots.Add(new Spot {
                Name = suggestion.Name, Address = suggestion.Address, City = suggestion.City,
                Category = suggestion.Category, Description = suggestion.Description,
                Latitude = suggestion.Latitude, Longitude = suggestion.Longitude,
                PricePerHour = suggestion.PricePerHour, Wifi = suggestion.Wifi,
                Outlets = suggestion.Outlets, Quiet = suggestion.Quiet,
                OpeningHours = suggestion.OpeningHours, Website = suggestion.Website,
                IsVerified = false
            });
        }
        suggestion.Status = approve ? "Approved" : "Rejected";
        suggestion.ReviewedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
