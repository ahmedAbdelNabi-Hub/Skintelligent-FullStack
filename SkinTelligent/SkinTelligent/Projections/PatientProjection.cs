using SkinTelIigent.Contracts.DTOs._Doctor;
using SkinTelIigent.Contracts.DTOs.Patient;
using System.Linq.Expressions;

namespace SkinTelligent.Api.Projections
{
    using System.Linq.Expressions;
    using Microsoft.Extensions.Configuration;
    using SkinTelIigent.Contracts.DTOs.Clinics;
    using SkinTelIigent.Contracts.DTOs.Patient;
    using SkinTelIigent.Core.Entities;

    public class PatientProjection
    {
        

        public Expression<Func<Doctor, IEnumerable<PatientDTO>>> ToPatientDTOs()
        {
            return doctor => doctor.DoctorPatients
                .Select(dp => new PatientDTO
                {
                    Id = dp.Patient.Id,
                    FirstName = dp.Patient.FirstName,
                    LastName = dp.Patient.LastName,
                    DateOfBirth = dp.Patient.DateOfBirth,
                    Gender = dp.Patient.Gender,
                    Address = dp.Patient.Address,
                    LastVisitDate = dp.Patient.LastVisitDate,
                    CreatedDate = dp.Patient.CreatedDate,
                    UpdatedDate = dp.Patient.UpdatedDate,
                    ProfilePicture = dp.Patient.ProfilePicture,
                    Email = dp.Patient.Email,
                    Phone = dp.Patient.Phone
                });
        }

    }

}
