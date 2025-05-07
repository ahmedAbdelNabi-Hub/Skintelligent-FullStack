using SkinTelIigent.Contracts.DTOs.Authentication;
using SkinTelIigent.Contracts.DTOs.Clinics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs._Doctor
{
    public class DoctorDTO 
    {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public double Rating { get; set; } 
            public string Gender { get; set; }
            public bool IsProfileCompleted { get;set; }
            public bool IsActive { get; set; }
            public string LicenseNumber { get; set; }
            public int ExperienceYears { get; set; }
            public string PhoneNumber { get; set; }
            
            public string Address { get; set; }
           
            public decimal DefaultExaminationFee { get; set; }
            public decimal DefaultConsultationFee { get; set; }
            public string ProfilePicture { get; set; }
            
            public string AboutMe { get; set; }
            public string Qualification { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime UpdatedDate { get; set; }
            public string Email { get; set; }
            public List<ClinicDTO> Clinics { get; set; }


    }

}

