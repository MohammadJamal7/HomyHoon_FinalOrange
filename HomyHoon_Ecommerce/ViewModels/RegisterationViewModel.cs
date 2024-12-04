using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HomyHoon_Ecommerce.ViewModels
{
    public class RegisterationViewModel
    {


        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [PasswordFormat] // Apply the custom password format validator
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Adress { get; set; }

       



    }

    public class PasswordFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrWhiteSpace(password))
            {
                return new ValidationResult("Password is required");
            }

            // Password validation rules
            if (password.Length < 8)
            {
                return new ValidationResult("Password must be at least 8 characters long");
            }

            if (!Regex.IsMatch(password, @"[A-Z]")) // At least one uppercase letter
            {
                return new ValidationResult("Password must contain at least one uppercase letter");
            }

            if (!Regex.IsMatch(password, @"[a-z]")) // At least one lowercase letter
            {
                return new ValidationResult("Password must contain at least one lowercase letter");
            }

            if (!Regex.IsMatch(password, @"[0-9]")) // At least one digit
            {
                return new ValidationResult("Password must contain at least one digit");
            }

            if (!Regex.IsMatch(password, @"[^A-Za-z0-9]")) // At least one special character
            {
                return new ValidationResult("Password must contain at least one special character");
            }

            return ValidationResult.Success;
        }
    }
}