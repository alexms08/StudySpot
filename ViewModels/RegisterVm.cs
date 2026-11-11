using System.ComponentModel.DataAnnotations;
namespace StudySpot.ViewModels;
public class RegisterVm {
 [Required,EmailAddress] public string Email {get;set;}="";
 [Required,MinLength(6),DataType(DataType.Password)] public string Password {get;set;}="";
 [Compare(nameof(Password)),DataType(DataType.Password)] public string ConfirmPassword {get;set;}="";
}
public class LoginVm { [Required,EmailAddress] public string Email {get;set;}=""; [Required,DataType(DataType.Password)] public string Password {get;set;}=""; public string? ReturnUrl {get;set;} }