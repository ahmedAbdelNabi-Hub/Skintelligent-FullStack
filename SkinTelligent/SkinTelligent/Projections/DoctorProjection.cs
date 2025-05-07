using SkinTelIigent.Contracts.DTOs._Doctor;
using SkinTelIigent.Contracts.DTOs.Clinics;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;

namespace SkinTelligent.Api.Projections
{
    public  class DoctorProjection
    {
        private readonly IConfiguration _configuration;

        public DoctorProjection(IConfiguration configuration)
        {
            _configuration = configuration; 
        }
        public  Expression<Func<Doctor, DoctorDTO>> ToDoctorDTO()
        {
            return d => new DoctorDTO
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                AboutMe = d.AboutMe,
                PhoneNumber = d.Phone!=null?d.Phone:"",
                ProfilePicture =!string.IsNullOrEmpty(d.ProfilePicture) ? $"{_configuration["ApiBaiseUrl"]}/image/doctorProfilePictures/{d.ProfilePicture}" : "",
                ExperienceYears = d.ExperienceYears,
                Address = d.Address,
                DateOfBirth = d.DateOfBirth, //
                Gender = d.Gender, //
                LicenseNumber =d.LicenseNumber, //
                Qualification = d.Qualification, //
                IsActive = d.IsApproved,
                Rating = d.Reviews.Any() ? d.Reviews.Average(r => r.Rating) : 0.0,
                DefaultConsultationFee = d.DefaultConsultationFee,
                DefaultExaminationFee = d.DefaultExaminationFee,
                CreatedDate = d.CreatedDate,
                UpdatedDate = d.UpdatedDate,
                Clinics = d.ClinicDoctors
                        .Select(cd => new ClinicDTO
                        {
                            id = cd.Clinic.Id,
                            ClinicName = cd.Clinic.ClinicName,
                            Address= cd.Clinic.Address,
                            ContactNumber = cd.Clinic.ContactNumber,
                            Image = !string.IsNullOrEmpty(cd.Clinic.Image) ? $"{_configuration["ApiBaiseUrl"]}/image/clinicProfilePictures/{cd.Clinic.Image}" : "",

                        }).ToList(),
                Email = d.Email!,
                IsProfileCompleted = d.IsProfileCompleted
            };
        }
    }
}
