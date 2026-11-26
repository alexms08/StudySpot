using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using StudySpot.Data;
using StudySpot.Models;
using StudySpot.ViewModels;

namespace StudySpot.Controllers;

[Authorize]
public class PlaceSuggestionsController(AppDbContext db) : Controller
{
    [HttpGet]
    public IActionResult Create() => View(new PlaceSuggestionVm());

    [HttpPost, ValidateAntiForgeryToken, EnableRateLimiting("submissions")]
    public async Task<IActionResult> Create(PlaceSuggestionVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (uid == null) return Challenge();
        db.PlaceSuggestions.Add(new PlaceSuggestion
        {
            UserId = uid, Name = vm.Name.Trim(), City = vm.City.Trim(), Address = vm.Address.Trim(),
            Category = vm.Category.Trim(), Description = vm.Description.Trim(), Website = vm.Website,
            Latitude = vm.Latitude, Longitude = vm.Longitude, PricePerHour = vm.PricePerHour,
            OpeningHours = vm.OpeningHours, Wifi = vm.Wifi, Outlets = vm.Outlets, Quiet = vm.Quiet
        });
        await db.SaveChangesAsync();
        TempData["Notice"] = "Предложението е изпратено за проверка.";
        return RedirectToAction(nameof(My));
    }

    [HttpGet]
    public async Task<IActionResult> My()
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return View(await db.PlaceSuggestions.AsNoTracking()
            .Where(s => s.UserId == uid).OrderByDescending(s => s.CreatedAt).ToListAsync());
    }
}
