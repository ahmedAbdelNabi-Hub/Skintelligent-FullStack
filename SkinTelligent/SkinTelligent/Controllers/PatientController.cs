using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Contracts.DTOs;
using SkinTelIigent.Contracts.DTOs._Appointment;
using SkinTelIigent.Contracts.DTOs._Doctor;
using SkinTelIigent.Contracts.DTOs.Patient;
using SkinTelIigent.Contracts.Interface;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Core.Specification;
using SkinTelIigent.Core.Specification.PatientSpecific;
using SkinTelIigent.Infrastructure.UnitOfWork;
using SkinTelIigent.Services;
using SkinTelIigentContracts.CustomResponses;
using SkinTelligent.Api.Extensions.DoctorExtensions;
using SkinTelligent.Api.Extensions.PatientExtensions;
using SkinTelligent.Api.Helper.MappingProfile;
using SkinTelligent.Api.Helper.Upload;
using SkinTelligent.Api.Hubs;

namespace SkinTelligent.Api.Controllers
{
    [Authorize]
    public class PatientController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IAppointmentService _appointmentService;
        private readonly INotificationSender _notificationSender;

        public PatientController(
          IConfiguration configuration,
          IUnitOfWork unitOfWork,
          IMapper mapper,
          IAppointmentService appointmentService,
          UserManager<ApplicationUser> userManager,
          INotificationSender notificationSender)
        {
            _unitOfWork = unitOfWork;
            _appointmentService = appointmentService;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _notificationSender = notificationSender; 
        }



        [Authorize]
        [HttpGet("/api/patients")]
        public async Task<ActionResult<PaginationDTO<PatientDTO>>> GetAllPatientsAsync([FromQuery] PaginationSpecParams specParams)
        {
            var spec = new PatientSpecification(specParams);
            var patients = await _unitOfWork.Repository<Patient>().GetQueryableWithSpec(spec).SelectPatientDTO().ToListAsync();
            if (patients == null || !patients.Any())
                return NotFound(new BaseApiResponse(404, "No patients found"));

            var totalCount = await _unitOfWork.Repository<Patient>().CountWithSpec(new PatientSpecification());

            return Ok(new PaginationDTO<PatientDTO>
            {
                data = patients,
                PageIndex = specParams.PageIndex,
                PageSize = specParams.PageSize,
                Count = totalCount
            });
        }

        [Authorize]
        [HttpGet("/api/patients/{id}")]
        public async Task<ActionResult<PatientDTO>> GetPatientById(int id)
        {
            var spec = new PatientSpecification(id);
            var patient = await _unitOfWork.Repository<Patient>().GetByIdSpecAsync(spec);

            if (patient is null)
                return NotFound(new BaseApiResponse(404, "Patient not found"));

            return Ok(_mapper.Map<PatientDTO>(patient));
        }
       
        
        [Authorize(Roles = "patient")]
        [HttpPut("/api/patients/update")]
        public async Task<ActionResult<BaseApiResponse>> UpdateDoctor([FromForm] UpdatePatientDTO patientDto)
        {
            var patientId = GetAuthenticatedPatientId();
            if (patientId == null)
                return Unauthorized(new BaseApiResponse(401, "Invalid or Unauthorized Patient."));

            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(patientId.Value);
            if (patient == null)
                return NotFound(new BaseApiResponse(404, "Patient not found."));

            var user = await _userManager.FindByIdAsync(patient.UserId);
            if (user == null)
                return NotFound(new BaseApiResponse(404, "Associated user not found."));

            PatientMapper.UpdatePatientData(patientDto, patient);
            user.PhoneNumber = patientDto.Phone;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return StatusCode(500, new BaseApiResponse(500, "Failed to update patient user info."));

            if (patientDto.ProfilePicture != null)
            {
                string folderName = "PatientPictures";
                patient.ProfilePicture = !string.IsNullOrEmpty(patient.ProfilePicture)
                    ? DocumentSettings.UpdateFile(patientDto.ProfilePicture, folderName, patient.ProfilePicture)
                    : DocumentSettings.UploadFile(patientDto.ProfilePicture, folderName);
            }

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.SaveChangeAsync();
            var isCommitted = await _unitOfWork.CommitAsync();

            return isCommitted
                ? Ok(new BaseApiResponse(200, "Patient updated successfully."))
                : StatusCode(500, new BaseApiResponse(500, "Failed to update patient."));
        }


        [Authorize(Roles = "patient")]
        [HttpGet("/api/patients/reports")]
        public async Task<ActionResult<IReadOnlyList<Report>>> GetPatientReports()
        {
            var patientId = GetAuthenticatedPatientId();
            if (patientId == null)
                return Unauthorized("Invalid or Unauthorized Patient.");

            var spec = new PatientSpecification().GetAllReportsByPatientId(patientId.Value);
            var patient = await _unitOfWork.Repository<Patient>().GetByIdSpecAsync(spec);
            if (patient == null)
                return NotFound("Reports not found");
            return Ok(patient.Reports);
        }


        [Authorize(Roles = "patient")]
        [HttpGet("/api/patients/doctors")]
        public async Task<ActionResult<PaginationDTO<DoctorDTO>>> GetPatientDoctors( [FromQuery] PaginationSpecParams paginationSpecParams)
        {
            var patientId = GetAuthenticatedPatientId();
            if (patientId == null)
                return Unauthorized("Invalid or Unauthorized Patient.");


            var apiBaseUrl = _configuration["ApiBaiseUrl"];
            var spec = new DoctorPatientsSpecification().GetPatientDoctors(patientId.Value, paginationSpecParams);
            var doctors = await _unitOfWork.Repository<DoctorPatient>()
                          .GetProjectedAsync(p => new DoctorDTO
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
                              ProfilePicture = !string.IsNullOrEmpty(p.Doctor.ProfilePicture) ? $"{apiBaseUrl}/image/doctorProfilePictures/{p.Doctor.ProfilePicture}" : "",
                              Qualification = p.Doctor.Qualification,
                              CreatedDate = p.Doctor.CreatedDate,
                              UpdatedDate = p.Doctor.UpdatedDate,
                          }, spec);   
            if(doctors == null || !doctors.Any())
                return NotFound(new BaseApiResponse(404, "No Doctors found"));

            return Ok(new PaginationDTO<DoctorDTO>
            {
                data = doctors,
                PageIndex = paginationSpecParams.PageIndex,
                PageSize = paginationSpecParams.PageSize,
                Count = 0
            });

        }

        [Authorize(Roles = "patient")]
        [HttpGet("/api/patients/appointments")]
        public async Task<ActionResult<IReadOnlyList<Appointment>>> GetBookedAppointmentsByPatient()
        {
            var patientId = GetAuthenticatedPatientId();
            if (patientId == null)
                return Unauthorized("Invalid or Unauthorized Patient.");

            var spec = new AppointmentSpecifications().GetBookedAppointmentsByPatientId(patientId.Value);
            var appointments = await _unitOfWork.Repository<Appointment>().GetAllWithSpecAsync(spec);

            if (appointments == null || !appointments.Any())
                return NotFound(new BaseApiResponse(404, "No booked appointments found"));

            var appointmentDTOs = _mapper.Map<IReadOnlyList<AppointmentDTO>>(appointments);
            return Ok(appointmentDTOs);
        }



        [Authorize(Roles = "patient")]
        [HttpPut("/api/patient/appointments/{id}/cancel")]
        public async Task<ActionResult<BaseApiResponse>> CancelAppointment(int id)
        {
            var patientId = GetAuthenticatedPatientId();
            if (patientId == null)
                return Unauthorized(new BaseApiResponse(401, "Invalid or Unauthorized Patient."));

            var result = await _appointmentService.CancelByPatinetAsync(id, patientId.Value);

            await _notificationSender.SendAsync(result.DoctorUserId, "A patient has cancelled an appointment.", result.DoctorNotificationCount!.Value);
            await _notificationSender.SendAsync(result.ClinicUserId, "A patient has cancelled an appointment at your clinic.", result.ClinicNotificationCount!.Value);

            return HandleStatusCode(result);
        }
        

        [Authorize(Roles = "patient")]
        [HttpDelete("/api/patients/appointments/{id}/delete")]
        public async Task<IActionResult> SoftDeleteByPatient(int id)
        {
            var patientId = GetAuthenticatedPatientId();
            if (patientId == null)
                return Unauthorized("Invalid or Unauthorized Patient.");

            var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(id);
            if (appointment == null)
                return NotFound(new BaseApiResponse(404, "Appointment not found"));

            if (appointment.PatientId != patientId)
                return Forbid("You are not allowed to deleted this appointment.");


            var now = DateTime.UtcNow;
            if (now < appointment.EndTime)
                return BadRequest(new BaseApiResponse(400, "You can't delete an appointment , Please Cancel It"));

            appointment.IsDeletedByPatient = true;
            appointment.PatientDeletedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangeAsync();

            return Ok(new BaseApiResponse(200, "Appointment as deleted "));
        }


        private int? GetAuthenticatedPatientId()
        {
            var patientIdClaim = User.FindFirst("patientId");

            if (patientIdClaim == null || !int.TryParse(patientIdClaim.Value, out int patientId))
                return null;

            return patientId;
        }
    }

}