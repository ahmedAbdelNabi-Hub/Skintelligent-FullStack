using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs.Clinics
{
    public class UpdateClinicDTO
    {

        [Required(ErrorMessage = "Clinic Name is required.")]
        [MaxLength(100, ErrorMessage = "Clinic Name cannot exceed 100 characters.")]
        public string ClinicName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Contact Number is required.")]
        [RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$", ErrorMessage = "Invalid Egyptian phone number.")]
        public string ContactNumber { get; set; }

        public IFormFile? Image { get; set; }
    }
}
