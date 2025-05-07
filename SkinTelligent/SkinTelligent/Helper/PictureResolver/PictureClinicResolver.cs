using AutoMapper;
using SkinTelIigent.Contracts.DTOs.Clinics;

namespace SkinTelligent.Api.Helper.PictureResolver
{
    public class PictureClinicResolver : IValueResolver<Clinic, ClinicDTO, string>
    {
        private readonly IConfiguration _configuration;

        public PictureClinicResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(Clinic source, ClinicDTO destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Image))
            {
                var pictureFileName = source.Image;
                return $"{_configuration["ApiBaiseUrl"]}/image/clinicProfilePictures/{pictureFileName}";
            }
            return ""; 
        }
    }
}
