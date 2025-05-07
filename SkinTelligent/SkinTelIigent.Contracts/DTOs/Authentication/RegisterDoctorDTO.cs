using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs.Authentication
{
    public class RegisterDoctorDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email format is invalid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; } = DateTime.Now;

        public string? Gender { get; set; } = "Not Specified";  

        public string? AboutMe { get; set; }=string.Empty;

        public string? LicenseNumber { get; set; } = "N/A";  

        public int ExperienceYears { get; set; } = 0;

        public decimal DefaultConsultationFee { get; set; } = 0m;

        public decimal DefaultExaminationFee { get; set; } = 0m;

        public string? ProfilePicture { get; set; } = "default.jpg";

        public string? Qualification { get; set; } = "Not Provided";  

        public string? Phone { get; set; } = string.Empty;

    }
}
