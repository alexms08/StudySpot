using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySpot.Data;
namespace StudySpot.Controllers;
public class HomeController(AppDbContext db) : Controller {
 public async Task<IActionResult> Index(string? q,string? city,string? category,bool wifi=false,bool outlets=false,bool quiet=false) {
  var s=db.Spots.Include(x=>x.Reviews).AsQueryable();
  if(!string.IsNullOrWhiteSpace(q)) s=s.Where(x=>x.Name.Contains(q)||x.Description.Contains(q)||x.Address.Contains(q));
  if(!string.IsNullOrWhiteSpace(city)) s=s.Where(x=>x.City.Contains(city));
  if(!string.IsNullOrWhiteSpace(category)) s=s.Where(x=>x.Category==category);
  if(wifi)s=s.Where(x=>x.Wifi); if(outlets)s=s.Where(x=>x.Outlets); if(quiet)s=s.Where(x=>x.Quiet);
  return View(await s.OrderBy(x=>x.Name).ToListAsync());
 }
 public IActionResult Error() { Response.StatusCode=500; return View("Status", 500); }
 public IActionResult Status(int code) { Response.StatusCode=code; return View(code); }
 public async Task<IActionResult> Details(int id) { var spot=await db.Spots.Include(x=>x.Reviews).ThenInclude(r=>r.User).FirstOrDefaultAsync(x=>x.Id==id); return spot==null?NotFound():View(spot); }
}