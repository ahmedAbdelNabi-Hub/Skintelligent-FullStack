using SkinTelIigent.Contracts.DTOs._Doctor;
using SkinTelIigent.Contracts.DTOs.Authentication;
using SkinTelIigent.Contracts.DTOs.Patient;
using SkinTelIigent.Core.Entities;

namespace SkinTelligent.Api.Helper.MappingProfile
{
    public static class PatientMapper
    {
        public static ApplicationUser ToApplicationUser(this RegisterPatientDTO dto,string ProfilePicture)
        {
            return new ApplicationUser
            {
                UserName = dto.Email.Split('@')[0],
                Email = dto.Email,
                UserType = "Patient",
                PhoneNumber=dto.Phone.Trim(),
                Patient = new Patient
                {
                    Email = dto.Email,  
                    Phone=dto.Phone.Trim(),
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender,
                    Address = dto.Address,  
                    ProfilePicture = ProfilePicture,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                }
            };
        }
        public static Patient UpdatePatientData(UpdatePatientDTO dto, Patient patient)
        {
            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.Address = dto.Address;
            patient.UpdatedDate = DateTime.UtcNow;

            return patient;
        }
    }
}
