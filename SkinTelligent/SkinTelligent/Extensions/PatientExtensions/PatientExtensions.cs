using SkinTelIigent.Contracts.DTOs.Patient;

namespace SkinTelligent.Api.Extensions.PatientExtensions
{
    public static class PatientExtensions
    {
        public static IQueryable<PatientDTO> SelectPatientDTO(this IQueryable<Patient> query)
        {
            return query.Select(p => new PatientDTO
            {
                Id=p.Id,
                Address = p.Address,
                DateOfBirth = p.DateOfBirth,
                ProfilePicture = p.ProfilePicture,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Gender = p.Gender
            });
        }
    }
}
