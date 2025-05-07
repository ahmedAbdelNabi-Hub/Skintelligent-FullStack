using SkinTelIigent.Contracts.DTOs.Authentication;
using SkinTelIigent.Core.Entities;

namespace SkinTelligent.Api.Helper.MappingProfile
{
    public static class ClinicMapper
    {
        public static ApplicationUser ToApplicationUser(this RegisterClinicDTO dto,string profileImageName)
        {
            
            
            return new ApplicationUser
            {
                UserName = dto.Email.Split('@')[0],
                Email = dto.Email,
                UserType = "Clinic",
                
                Clinic = new Clinic
                {
                    ClinicName = dto.ClinicName,
                    Address = dto.Address,
                    Image = profileImageName,
                    ContactNumber = dto.ContactNumber,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                }
            };
        }
    }
}
