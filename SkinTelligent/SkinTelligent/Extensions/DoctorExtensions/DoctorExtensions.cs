using SkinTelIigent.Contracts.DTOs._Doctor;
using SkinTelIigent.Contracts.DTOs.Patient;
using SkinTelIigent.Core.Entities;

namespace SkinTelligent.Api.Extensions.DoctorExtensions
{
    public static class DoctorExtensions
    {
        public static IQueryable<DoctorDTO> SelectDoctorDTO(this IQueryable<DoctorPatient> query)
        {
            return query.Select(p => new DoctorDTO
            {
                Id = p.Id,
                FirstName = p.Doctor.FirstName,
                LastName = p.Doctor.LastName,
                DateOfBirth = p.Doctor.DateOfBirth,
                Gender = p.Doctor.Gender,
                LicenseNumber = p.Doctor.LicenseNumber,
                ExperienceYears = p.Doctor.ExperienceYears,
                DefaultExaminationFee = p.Doctor.DefaultExaminationFee,
                DefaultConsultationFee = p.Doctor.DefaultConsultationFee,
                ProfilePicture = p.Doctor.ProfilePicture,
                Qualification = p.Doctor.Qualification,
                CreatedDate = p.Doctor.CreatedDate,
                UpdatedDate = p.Doctor.UpdatedDate,                
            });
        }
    }
}
