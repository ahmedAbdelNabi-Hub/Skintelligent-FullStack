using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkinTelIigent.Contracts.DTOs.Authentication;
using SkinTelIigent.Contracts.DTOs._Doctor;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Contracts.DTOs.Patient;
using SkinTelIigent.Contracts.DTOs.Clinics;
using SkinTelIigent.Contracts.DTOs._Appointment;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelligent.Api.Helper.PictureResolver;
using SkinTelIigent.Contracts.DTOs.Notification;

namespace SkinTelligent.Api.Helper.MappingProfile
{ 
    public class MappingProfile  : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDoctorDTO, Doctor>()
                        .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                        .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<RegisterDoctorDTO, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<Doctor, DoctorDTO>()
                           .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                            .ForMember(dest=>dest.IsActive,srcs=>srcs.MapFrom(scr=>scr.IsApproved))
                           .ForMember(dest => dest.Rating, opt =>opt.MapFrom(src => src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
                           .ForMember(dest=>dest.Clinics,opt=>opt.MapFrom(src=>src.ClinicDoctors.Select(c=>c.Clinic)))
                           .ForMember(dest=>dest.ProfilePicture,opt=>opt.MapFrom<PictureDoctorResolver>());
                         
                

            CreateMap<Patient,UpdatePatientDTO>()
                .ReverseMap();

            CreateMap<Clinic, ClinicDTO>()
                 .ForMember(s => s.Email, opt => opt.MapFrom(d => d.User.Email)) 
                 .ForMember(dest => dest.Image, opt => opt.MapFrom<PictureClinicResolver>())
                 .ReverseMap();

            CreateMap<UpdateDoctorDTO, Doctor>();

            CreateMap<Doctor, DoctorWithPatientsDTO>()
                 .ForMember(dest => dest.Patients, opt => opt.MapFrom(src => src.DoctorPatients.Select(dp => dp.Patient)));

            CreateMap<Patient, PatientDTO>()
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom<PicturePatientResolver>());

            CreateMap<DoctorPatient, PatientDTO>();

            CreateMap<Doctor, DoctorWithClinicsDTO>()
          .ForMember(dest => dest.Clinics, opt => opt.MapFrom(src => src.ClinicDoctors.Select(cd => cd.Clinic).ToList()));


            CreateMap<Appointment, AppointmentDTO>()
               .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? $"{src.Doctor.FirstName} {src.Doctor.LastName}" : ""))
                .ForMember(dest => dest.ClinicAddress, opt => opt.MapFrom(src => src.Clinic != null ? src.Clinic.Address : ""))
               .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic != null ? src.Clinic.ClinicName : ""));

            CreateMap<Notification,NotificationDTO>().ReverseMap();
                
        
        }
    }
}
