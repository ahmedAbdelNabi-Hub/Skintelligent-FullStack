using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Contracts.DTOs;
using SkinTelIigent.Contracts.DTOs._Appointment;
using SkinTelIigent.Contracts.DTOs._Doctor;
using SkinTelIigent.Contracts.DTOs.Clinics;
using SkinTelIigent.Contracts.DTOs.Patient;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Core.Specification;
using SkinTelIigent.Infrastructure.Data;
using SkinTelIigentContracts.CustomResponses;
using SkinTelligent.Api.Helper.Upload;

namespace SkinTelligent.Api.Controllers
{
  
    public class ClinicController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        private readonly UserManager<ApplicationUser> _userManager;


        public ClinicController(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _configuration = configuration;
        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("/api/clinics")]
        public async Task<ActionResult<PaginationDTO<ClinicWithDoctorDTO>>> GetAllClinicsWithDoctorEmails([FromQuery] PaginationSpecParams paginationParams)
        {
            var countAysnc = await _unitOfWork.Repository<Clinic>().CountWithSpec(new ClinicSpecifications().CountApprovedClinic());
            if(countAysnc == 0)
                return NotFound(new BaseApiResponse(StatusCodes.Status404NotFound, "Not Found Clinic"));

            var clinics = await _unitOfWork.Repository<Clinic>().GetProjectedAsync(cd => new ClinicWithDoctorDTO
                {
                    id = cd.Id,
                    ClinicName = cd.ClinicName,
                    Address = cd.Address,
                    ContactNumber = cd.ContactNumber,
                    CreatedDate = cd.CreatedDate,
                    Email = cd.User.Email!,
                    Image = !string.IsNullOrEmpty(cd.Image) ? $"{_configuration["ApiBaiseUrl"]}/image/clinicProfilePictures/{cd.Image}" : "",
                    Emails = cd.ClinicDoctors
                    .Select(cd => cd.Doctor.User.Email)
                    .Distinct()
                    .ToList()!
                },
                new ClinicSpecifications(paginationParams)
            );
            
            if(clinics is  null)
                return NotFound(new BaseApiResponse(StatusCodes.Status404NotFound,"Not Found Clinic"));

            return Ok(new PaginationDTO<ClinicWithDoctorDTO> { data=clinics,PageIndex=paginationParams.PageIndex,PageSize=paginationParams.PageSize,Count=countAysnc});
        }

        [Authorize(Roles = "admin")]
        [HttpPatch]
        [Route("/api/clinics/{id}/approve")]
        public async Task<ActionResult<BaseApiResponse>> Approveclinic(int id)
        {
            var clinic = await _unitOfWork.Repository<Clinic>().GetByIdAsync(id);
            if (clinic == null)
            {
                return NotFound(new BaseApiResponse(404, "Clinic not found"));
            }
            await _unitOfWork.BeginTransactionAsync();
            clinic.IsApproved = true;
            clinic.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.SaveChangeAsync();
            var isUpdated =  await _unitOfWork.CommitAsync();
            if (isUpdated) return Ok(new BaseApiResponse(200, "Clinic Account approved successfully."));
            else return StatusCode(StatusCodes.Status500InternalServerError,new BaseApiResponse(500, "Failed to approve Clinic"));
        }


        [Authorize(Roles = "clinic")]
        [HttpGet("/api/clinics/doctors")]
        public async Task<ActionResult<PaginationDTO<DoctorDTO>>> GetDoctorsByClinicId([FromQuery] DoctorFilterParams filterParams)
        {
            var clinicId = GetAuthenticatedClinicId();
            if (clinicId == null)
                return Unauthorized("Invalid or Unauthorized Clinic.");

            var clinicDoctorRepo = _unitOfWork.Repository<ClinicDoctor>();

            var specification = new ClinicDoctorSpecifications()
                .GetDoctorsByClinicId(clinicId.Value, filterParams);

            var clinicDoctors = await clinicDoctorRepo.GetAllWithSpecAsync(specification);
            if (clinicDoctors is null || !clinicDoctors.Any())
                return NotFound(new BaseApiResponse(StatusCodes.Status404NotFound, "No doctors found for this clinic."));

            var countSpec = new ClinicDoctorSpecifications(clinicId.Value, filterParams); 
            var count = await clinicDoctorRepo.CountWithSpec(countSpec);

            var mappedDoctors = _mapper.Map<IReadOnlyList<DoctorDTO>>(clinicDoctors.Select(cd => cd.Doctor));

            return Ok(new PaginationDTO<DoctorDTO>
            {
                Count = count,
                data = mappedDoctors,
                PageIndex = filterParams.PageIndex,
                PageSize = filterParams.PageSize
            });
        }



        [HttpGet]
        [Route("/api/clinics/{id}")]
        public async Task<ActionResult<ClinicDTO>> GetClinicWithDoctors(int id)
        {
            var clinicRepo = _unitOfWork.Repository<Clinic>();
            var specification = new ClinicSpecifications(id);

            var Clinic = await clinicRepo.GetByIdSpecAsync(specification);
            if (Clinic == null) return NotFound(new BaseApiResponse(StatusCodes.Status404NotFound, "Clinic not found"));

            var clinicDTO = _mapper.Map<ClinicDTO>(Clinic);

            return Ok(clinicDTO);
        }

        [Authorize(Roles ="clinic")]
        [HttpPut]
        [Route("/api/clinics")]
        public async Task<ActionResult<BaseApiResponse>> Updateclinic([FromForm] UpdateClinicDTO data)
        {
            var clinicId = GetAuthenticatedClinicId();
            if (clinicId == null)
                return Unauthorized("Invalid or Unauthorized Patient.");

            var clinic = await _unitOfWork.Repository<Clinic>().GetByIdAsync(clinicId.Value);
            if (clinic is null)
                return NotFound(new BaseApiResponse(404, "Clinic not found"));
            
            _mapper.Map(data, clinic);

            if (data.Image != null)
            {
                string folderName = "ClinicPictures";
                clinic.Image = !string.IsNullOrEmpty(clinic.Image)
                    ? DocumentSettings.UpdateFile(data.Image, folderName, clinic.Image)
                    : DocumentSettings.UploadFile(data.Image, folderName);
            }
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.SaveChangeAsync();
            var isUpdated = await _unitOfWork.CommitAsync();
            if (isUpdated)
                return Ok(new BaseApiResponse(200, "Clinic updated successfully."));
            else
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseApiResponse(500, "Failed to update Clinic"));
        }


        [Authorize(Roles ="admin")]
        [HttpGet("/api/clinics/not-approved")]
        public async Task<ActionResult<PaginationDTO<ClinicDTO>>> GetNotApprovedClinics([FromQuery] PaginationSpecParams paginationParams)
        {
            var clinicRepository = _unitOfWork.Repository<Clinic>();
            var specification = new ClinicSpecifications().GetNotApprovedClinics(paginationParams);

            var clinics = await clinicRepository.GetAllWithSpecAsync(specification);
            if (clinics == null || !clinics.Any())
            {
                return NotFound(new BaseApiResponse(404, "No unapproved clinics found."));
            }

            var totalCount = await clinicRepository.CountWithSpec(specification);
            var mappedClinics = _mapper.Map<IReadOnlyList<ClinicDTO>>(clinics);

            var paginationResult = new PaginationDTO<ClinicDTO>
            {
                data = mappedClinics,
                PageIndex = paginationParams.PageIndex,
                PageSize = paginationParams.PageSize,
                Count = totalCount
            };

            return Ok(paginationResult);
        }


        [Authorize(Roles = "clinic")]
        [HttpGet("/api/clinics/appointments")]
        public async Task<ActionResult<BaseApiResponse>> GetClinicAppointments([FromQuery] DateTime date)
        {
            var clinicId = GetAuthenticatedClinicId();
            if (clinicId == null)
                return Unauthorized("Invalid or Unauthorized Patient.");


            var appointmentSpec = new AppointmentSpecifications().GetClinicAppointments(clinicId.Value, date);
            var appointments = await _unitOfWork.Repository<Appointment>().GetProjectedAsync(
                a => new AppointmentSlotDTO
                {
                    Id = a.Id,
                    TimeSlot = $"{a.StartTime:hh:mm tt} - {a.EndTime:hh:mm tt}",
                    IsBooked = a.PatientId != null,

                    Doctor = new DoctorDTO
                    {
                        Id = a.Doctor.Id,
                        FirstName = a.Doctor.FirstName,
                        LastName = a.Doctor.LastName,
                        ProfilePicture = a.Doctor.ProfilePicture,
                        DefaultExaminationFee = a.Doctor.DefaultExaminationFee,
                        AboutMe = a.Doctor.AboutMe,

                    },
                    Patient = a.PatientId != null ? new PatientDTO
                    {
                        Id = a.Patient!.Id,
                        FirstName = a.Patient.FirstName,
                        LastName = a.Patient.LastName,
                        Address = a.Patient.Address

                    } : null
                },
                appointmentSpec
            );
            return Ok(appointments);
        }

        private int? GetAuthenticatedClinicId()
        {
            var clinicIdClaim = User.FindFirst("clinicId");

            if (clinicIdClaim == null || !int.TryParse(clinicIdClaim.Value, out int clinicId))
                return null;

            return clinicId;
        }



    }
}
