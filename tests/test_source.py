"""Offline structural checks; not a substitute for dotnet test."""
from pathlib import Path
import unittest
P=Path(__file__).resolve().parents[1]
class StudySpotSourceTests(unittest.TestCase):
 def read(self,f): return (P/f).read_text(encoding="utf-8")
 def test_mvc_routes(self): self.assertIn("MapControllerRoute",self.read("Program.cs"))
 def test_auth(self): self.assertIn("AddIdentity<AppUser, IdentityRole>",self.read("Program.cs"));self.assertIn("UseAuthentication",self.read("Program.cs"))
 def test_admin_protection(self): self.assertIn('[Authorize(Roles="Admin")]',self.read("Controllers/AdminController.cs"))
 def test_mutation_csrf(self):
  for f in ["Controllers/AccountController.cs","Controllers/AdminController.cs","Controllers/FavoritesController.cs","Controllers/ReviewsController.cs"]: self.assertIn("ValidateAntiForgeryToken",self.read(f))
 def test_unique_reviews_and_favorites(self):
  s=self.read("Data/AppDbContext.cs");self.assertIn("HasIndex",s);self.assertEqual(s.count("IsUnique()"),2)
 def test_database_and_seed(self): self.assertIn("UseSqlite",self.read("Program.cs"));self.assertIn("EnsureCreatedAsync",self.read("Data/Seed.cs"))
 def test_input_validation(self): self.assertIn("[Range(1,5)]",self.read("Models/Review.cs"));self.assertIn("[Required,StringLength(100)]",self.read("Models/Spot.cs"))
 def test_views_exist(self):
  for f in ["Home/Index","Home/Details","Home/Status","Account/Login","Account/Register","Admin/Index","Admin/Edit","Favorites/Index"]: self.assertTrue((P/"Views"/(f+".cshtml")).exists())
 def test_no_public_admin_password(self): self.assertNotIn("Admin123",self.read("Data/Seed.cs"))
if __name__=="__main__": unittest.main()
