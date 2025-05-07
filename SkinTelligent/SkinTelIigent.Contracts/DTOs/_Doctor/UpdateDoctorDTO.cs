using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs._Doctor
{
    public class UpdateDoctorDTO
    {
        [Required(ErrorMessage = "FirstName  is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName  is required.")]

        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "About Me is required.")]
        public string AboutMe { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Consultation Fee is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Consultation Fee must be a positive number.")]
        public decimal DefaultExaminationFee { get; set; }

        [Required(ErrorMessage = "Consultation Fee is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Consultation Fee must be a positive number.")]
        public decimal DefaultConsultationFee { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "License Number is required.")]
        public string LicenseNumber { get; set; }

        [Required(ErrorMessage = "Experience Years is required.")]
        [Range(0, 100, ErrorMessage = "Experience Years must be between 0 and 100.")]
        public int ExperienceYears { get; set; }


        [Required]
        public IFormFile ProfilePicture { get; set; }

        [Required(ErrorMessage = "Qualification is required.")]
        public string Qualification { get; set; }

    }
}
