using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySpot.Data;

namespace StudySpot.Controllers;

public class HomeController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? q, string? city, string? category,
        bool wifi = false, bool outlets = false, bool quiet = false,
        string sort = "name", int minRating = 0, int page = 1)
    {
        page = Math.Clamp(page, 1, 10000);
        minRating = Math.Clamp(minRating, 0, 5);
        var spots = db.Spots.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var search = q.Trim();
            spots = spots.Where(x => x.Name.Contains(search) || x.Description.Contains(search) || x.Address.Contains(search));
        }
        if (!string.IsNullOrWhiteSpace(city)) spots = spots.Where(x => x.City.Contains(city.Trim()));
        if (!string.IsNullOrWhiteSpace(category)) spots = spots.Where(x => x.Category == category);
        if (wifi) spots = spots.Where(x => x.Wifi);
        if (outlets) spots = spots.Where(x => x.Outlets);
        if (quiet) spots = spots.Where(x => x.Quiet);
        if (minRating > 0) spots = spots.Where(x => x.Reviews.Any() && x.Reviews.Average(r => r.Rating) >= minRating);
        var total = await spots.CountAsync();
        spots = sort switch
        {
            "rating" => spots.OrderByDescending(x => x.Reviews.Any() ? x.Reviews.Average(r => r.Rating) : 0).ThenBy(x => x.Id),
            "price" => spots.OrderBy(x => x.PricePerHour).ThenBy(x => x.Id),
            "newest" => spots.OrderByDescending(x => x.Id),
            _ => spots.OrderBy(x => x.Name).ThenBy(x => x.Id)
        };
        const int pageSize = 12;
        ViewBag.Page = page;
        ViewBag.Total = total;
        ViewBag.Pages = (int)Math.Ceiling(total / (double)pageSize);
        return View(await spots.Include(x => x.Reviews).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());
    }

    public IActionResult Error() { Response.StatusCode = 500; return View("Status", 500); }
    public IActionResult Status(int code) { Response.StatusCode = code; return View("Status", code); }

    public async Task<IActionResult> Details(int id)
    {
        var spot = await db.Spots.AsNoTracking().Include(x => x.Reviews).ThenInclude(r => r.User)
            .FirstOrDefaultAsync(x => x.Id == id);
        return spot == null ? NotFound() : View(spot);
    }
}
