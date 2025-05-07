using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SkinTelIigent.Core.Entities;
namespace SkinTelIigent.Contracts.DTOs.Patient
{
    public class UpdatePatientDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [MaxLength(11, ErrorMessage = "Phone Must be 11 number")]
        [MinLength(11, ErrorMessage = "Phone Must be 11 number")]
        public string Phone { get; set; }

        public IFormFile ProfilePicture { get; set; }
 
    }
}
