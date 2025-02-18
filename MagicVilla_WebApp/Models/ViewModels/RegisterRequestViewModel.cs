using System.ComponentModel.DataAnnotations;

namespace MagicVilla_WebApp.Models.ViewModels
{
    public class RegisterRequestViewModel
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Compare(nameof(Password), ErrorMessage = "must match password")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
