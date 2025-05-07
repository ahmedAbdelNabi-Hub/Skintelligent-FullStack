using AutoMapper;
using Microsoft.Extensions.Configuration;
using SkinTelIigent.Contracts.DTOs._Doctor;

namespace SkinTelligent.Api.Helper.PictureResolver
{
    public class PictureDoctorResolver : IValueResolver<Doctor, DoctorDTO, string>
    {
        private readonly IConfiguration _configuration;

        public PictureDoctorResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(Doctor source, DoctorDTO destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ProfilePicture))
            {
                var pictureFileName = source.ProfilePicture;
                return $"{_configuration["ApiBaiseUrl"]}/image/doctorProfilePictures/{pictureFileName}";
            }

            return "";
        }
    }
}
