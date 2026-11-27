using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StudySpot.Data;
using StudySpot.Models;
using StudySpot.ViewModels;
using Xunit;

namespace StudySpot.Tests;

public class ModelAndDatabaseTests
{
    [Fact]
    public void PlaceSuggestionStartsPending()
    {
        var suggestion = new PlaceSuggestion { Name = "Library", City = "Varna", Address = "Center" };
        Assert.Equal("Pending", suggestion.Status);
        Assert.True(suggestion.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void InvalidSpotNameIsRejected()
    {
        var spot = new Spot { Name = "", Address = "Example Street 1", City = "Sofia" };
        var results = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(spot, new ValidationContext(spot), results, true));
        Assert.Contains(results, x => x.MemberNames.Contains(nameof(Spot.Name)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void RatingsOutsideOneToFiveAreInvalid(int rating)
    {
        var review = new Review { Rating = rating, Comment = "Test" };
        var results = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(review, new ValidationContext(review), results, true));
        Assert.Contains(results, x => x.MemberNames.Contains(nameof(Review.Rating)));
    }

    [Fact]
    public void MissingSuggestionAddressIsInvalid()
    {
        var vm = new PlaceSuggestionVm { Name = "Test", City = "Burgas", Address = "" };
        var results = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(vm, new ValidationContext(vm), results, true));
    }

    [Fact]
    public void FavoriteUserSpotPairMustBeUnique()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        using var db = new AppDbContext(options);
        db.Database.EnsureCreated();
        var user = new AppUser { Id = "test-user", UserName = "student", NormalizedUserName = "STUDENT" };
        var spot = new Spot { Name = "Test library", City = "Sofia", Address = "Example address" };
        db.Users.Add(user);
        db.Spots.Add(spot);
        db.SaveChanges();
        db.Favorites.Add(new Favorite { UserId = user.Id, SpotId = spot.Id });
        db.SaveChanges();
        db.Favorites.Add(new Favorite { UserId = user.Id, SpotId = spot.Id });
        Assert.Throws<DbUpdateException>(() => db.SaveChanges());
    }
}
