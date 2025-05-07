using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkinTelIigent.Core.Entities;
namespace SkinTelIigent.Contracts.DTOs.Patient
{
    public class PatientDTO
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Address { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        [Required]
        [EmailAddress(ErrorMessage ="email is not vaild")]
        public string Email { get; set; }

        [Required]
  
        public string Phone { get; set; }





    }
}
