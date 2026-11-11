using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudySpot.Models;
using StudySpot.ViewModels;
namespace StudySpot.Controllers;
public class AccountController(UserManager<AppUser> users,SignInManager<AppUser> signIn) : Controller {
 [HttpGet] public IActionResult Register()=>View(new RegisterVm());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Register(RegisterVm vm) {
  if(!ModelState.IsValid)return View(vm);
  var user=new AppUser{UserName=vm.Email,Email=vm.Email}; var result=await users.CreateAsync(user,vm.Password);
  if(result.Succeeded){await signIn.SignInAsync(user,false);return RedirectToAction("Index","Home");}
  foreach(var e in result.Errors)ModelState.AddModelError("",e.Description);return View(vm);
 }
 [HttpGet] public IActionResult Login(string? returnUrl=null)=>View(new LoginVm{ReturnUrl=returnUrl});
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Login(LoginVm vm) {
  if(!ModelState.IsValid)return View(vm);
  var result=await signIn.PasswordSignInAsync(vm.Email,vm.Password,false,lockoutOnFailure:true);
  if(result.Succeeded)return LocalRedirect(Url.IsLocalUrl(vm.ReturnUrl)?vm.ReturnUrl!:"/");
  ModelState.AddModelError("","Невалиден имейл или парола.");return View(vm);
 }
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Logout(){await signIn.SignOutAsync();return RedirectToAction("Index","Home");}
}