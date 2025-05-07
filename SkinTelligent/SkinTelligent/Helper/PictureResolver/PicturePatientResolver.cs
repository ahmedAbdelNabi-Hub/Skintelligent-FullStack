using AutoMapper;
using SkinTelIigent.Contracts.DTOs._Doctor;
using SkinTelIigent.Contracts.DTOs.Patient;

namespace SkinTelligent.Api.Helper.PictureResolver
{
    public class PicturePatientResolver: IValueResolver<Patient, PatientDTO, string>
    {
        private readonly IConfiguration _configuration;

        public PicturePatientResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(Patient source, PatientDTO destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ProfilePicture))
            {
                var pictureFileName = source.ProfilePicture;
                return $"{_configuration["ApiBaiseUrl"]}/image/PatientPictures/{pictureFileName}";
            }

            return "";
        }
    }
}
