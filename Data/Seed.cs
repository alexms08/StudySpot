using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudySpot.Models;
namespace StudySpot.Data;
public static class Seed {
 public static async Task Initialize(IServiceProvider services) {
  var db=services.GetRequiredService<AppDbContext>(); await db.Database.EnsureCreatedAsync();
  var roles=services.GetRequiredService<RoleManager<IdentityRole>>(); if(!await roles.RoleExistsAsync("Admin")) await roles.CreateAsync(new IdentityRole("Admin"));
  if(await db.Spots.AnyAsync()) return;
  db.Spots.AddRange(
   new Spot{Name="Примерна градска библиотека",City="Варна",Address="Демонстрационен адрес 1",Category="Библиотека",Description="Примерен запис за демонстрация. Не е проверено реално място.",Latitude=43.207,Longitude=27.916,Wifi=true,Outlets=true,Quiet=true,OpeningHours="Проверете на място"},
   new Spot{Name="Примерно учебно кафене",City="Варна",Address="Демонстрационен адрес 2",Category="Кафене",Description="Демонстрационни данни, не е реална препоръка.",Latitude=43.210,Longitude=27.925,Wifi=true,Outlets=true,PricePerHour=5,OpeningHours="Проверете на място"},
   new Spot{Name="Примерен coworking център",City="София",Address="Демонстрационен адрес 3",Category="Coworking",Description="Примерно място за тестване на филтрите.",Latitude=42.6977,Longitude=23.3219,Wifi=true,Outlets=true,Quiet=true,PricePerHour=10,OpeningHours="Проверете на място"});
  await db.SaveChangesAsync();
 }
}