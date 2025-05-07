using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs.Authentication
{
    public class ConfirmEmailDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email format is invalid.")]
        public string Email { get; set; }


        [Required]
        [MaxLength(6,ErrorMessage ="The OTP code must be 6 numbers")]
        [MinLength(6,ErrorMessage = "The OTP code must be 6 numbers")]
        public string OtpCode { get; set; }    
    }
}
