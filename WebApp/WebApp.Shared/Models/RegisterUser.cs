using System.ComponentModel.DataAnnotations;

namespace WebApp.Shared.Models
{
    public class RegisterUser
    {
        [Required] 
        public int ID { get; set; }

        [Required]
        [RegularExpression("^[A-Z]{1}[a-z]{1,}$", ErrorMessage = "The first letter of Fist Name must be capital and minimum length is 2")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression("^[A-Z]{1}[a-z]{1,}$", ErrorMessage = "The first letter of Last Name must be capital and minimum length is 2")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required] 
        public string UserName { get; set; }

        [Required] 
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{8,}$", ErrorMessage = "Password must have at least 8 characters and contain at least one lowercase letter, one uppercase letter and at least one number")] 
        public string Password { get; set; }

        [Required] 
        [Compare(nameof(Password), ErrorMessage = "The password and confirm password fields do not match")] 
        public string ConfirmPassword { get; set; }

        [Required] 
        public string Role { get; set; } = "user";
    }
}
