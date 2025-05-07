using SkinTelIigent.Contracts.DTOs._Doctor;
using SkinTelIigent.Contracts.DTOs.Authentication;
using SkinTelIigent.Core.Entities;

namespace SkinTelligent.Api.Helper.MappingProfile
{
    public static class DoctorMapper
    {

        public static ApplicationUser ToApplicationUser(RegisterDoctorDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email.Split('@')[0],
                Email = dto.Email,
                PhoneNumber = dto.Phone,
                UserType = "Doctor",
                Doctor = new Doctor
                {
                    Email = dto.Email,
                    Phone = dto.Phone!.Trim(),
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender,
                    LicenseNumber = dto.LicenseNumber,
                    AboutMe = dto.AboutMe,
                    Address = string.Empty,
                    ExperienceYears = dto.ExperienceYears,
                    DefaultConsultationFee = dto.DefaultConsultationFee,
                    DefaultExaminationFee = dto.DefaultExaminationFee,
                    ProfilePicture = dto.ProfilePicture,
                    Qualification = dto.Qualification,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                }
            };

            user.GetType().GetProperty("Discriminator")?.SetValue(user, "Doctor"); 

            return user;
        }
        public static Doctor UpdateDoctorData(UpdateDoctorDTO dto , Doctor doctor)
        {
                     doctor.FirstName = dto.FirstName;
                     doctor.LastName = dto.LastName;
                     doctor.DateOfBirth = dto.DateOfBirth;
                     doctor.Gender = dto.Gender;
                     doctor.Address = dto.Address;
                     doctor.LicenseNumber = dto.LicenseNumber;
                     doctor.AboutMe = dto.AboutMe;
                     doctor.ExperienceYears = dto.ExperienceYears;
                     doctor.DefaultConsultationFee = dto.DefaultConsultationFee;
                     doctor.DefaultExaminationFee = dto.DefaultExaminationFee;
                     doctor.Qualification = dto.Qualification;
                     doctor.CreatedDate = DateTime.UtcNow;
                     doctor.UpdatedDate = DateTime.UtcNow;

            return doctor;
        }

    }
}
