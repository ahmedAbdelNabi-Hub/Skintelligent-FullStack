using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Contracts.DTOs;
using SkinTelIigent.Contracts.DTOs._Doctor;
using SkinTelIigent.Contracts.DTOs.Authentication;
using SkinTelIigent.Contracts.DTOs.Clinics;
using SkinTelIigent.Contracts.DTOs.Patient;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Core.Specification;
using SkinTelIigentContracts.CustomResponses;
using SkinTelligent.Api.Helper.MappingProfile;
using SkinTelligent.Api.Helper.Upload;
using SkinTelligent.Api.Projections;
using System.Data;
using System.Numerics;

namespace SkinTelligent.Api.Controllers
{
    public class DoctorController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;



        public DoctorController(IConfiguration configuration,IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _configuration = configuration;
        }

   
        [HttpGet]
        [Route("/api/doctors")]
        public async Task<ActionResult<PaginationDTO<DoctorDTO>>> GetAllDoctors([FromQuery] PaginationSpecParams _params,[FromQuery] bool? Recently = false)
        {
            var doctorRepo = _unitOfWork.Repository<Doctor>();
            var apiBaseUrl = _configuration["ApiBaiseUrl"];

            ISpecification<Doctor> doctorSpec;
            ISpecification<Doctor> doctorCountSpec;
            if (Recently == true)
            {
                doctorSpec = new DoctorSpecifications(_params, includeOnlyRecent: true);
                doctorCountSpec = new DoctorSpecifications(includeOnlyRecent: true);
            }
            else
            {
                doctorSpec = new DoctorSpecifications(_params); 
                doctorCountSpec = new DoctorSpecifications().CountDoctorCompletedBeforePagination();   
            }

            var count = await doctorRepo.CountWithSpec(doctorCountSpec);

            var doctors = await doctorRepo.GetProjectedAsync(d => new DoctorDTO
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                ExperienceYears = d.ExperienceYears,
                IsProfileCompleted=d.IsProfileCompleted,
                ProfilePicture = !string.IsNullOrEmpty(d.ProfilePicture) ? $"{apiBaseUrl}/image/doctorProfilePictures/{d.ProfilePicture}" : "",
                DefaultExaminationFee = d.DefaultExaminationFee,
                Address = d.Address,
                Qualification = d.Qualification,
                LicenseNumber = d.LicenseNumber ,
                Rating = d.Reviews.Any() ? d.Reviews.Average(d => d.Rating) : 0,
                Clinics = d.ClinicDoctors.Select(cd => new ClinicDTO
                {
                    id = cd.Clinic.Id,
                    ClinicName = cd.Clinic.ClinicName,
                    Address = cd.Clinic.Address
                }).ToList()
            }, doctorSpec);

            if (doctors == null || !doctors.Any())
                return NotFound(new BaseApiResponse(404,"Not Found Doctors"));

            return Ok(new PaginationDTO<DoctorDTO>
            {
                data = doctors,
                PageIndex = _params.PageIndex,
                PageSize = _params.PageSize,
                Count = count
            });
        }

        [Authorize(Roles = "doctor,patient")]
        [HttpGet]
        [Route("/api/doctors/{id}")]
        public async Task<ActionResult<DoctorDTO>> GetDoctorById(int? id)
        {
            int? doctorId = id!.Value;
            if (User.IsInRole("doctor"))
            {
                doctorId = GetAuthenticatedDoctorId();
                if (doctorId == null)
                    return Unauthorized(new BaseApiResponse(401, "Invalid or Unauthorized Doctor."));
            }
            else if (User.IsInRole("patient"))
                doctorId = id;


            var doctorRepo = _unitOfWork.Repository<Doctor>();
            var doctor = await doctorRepo.GetProjectedAsync(
                selector: new DoctorProjection(_configuration).ToDoctorDTO(),
                spec: new DoctorSpecifications(doctorId.Value)
            );
            if (!doctor.Any())
                return NotFound(new BaseApiResponse(404, "Doctor not found"));

            return Ok(doctor);
        }

        [Authorize(Roles = "clinic")]
        [HttpPost("/api/doctors")]
        public async Task<ActionResult<BaseApiResponse>> CreateOrAssignDoctor([FromBody] RegisterDoctorDTO registerDoctorDTO)
        {
            var clinicId = GetAuthenticatedClinicId();
            if (clinicId == null)
                return Unauthorized("Invalid or Unauthorized Clinic.");

            var clinic = await _unitOfWork.Repository<Clinic>().GetByIdAsync(clinicId.Value);
            if (clinic == null)
                return BadRequest(new BaseApiResponse(400, "Clinic not found."));

            var user = await _userManager.FindByEmailAsync(registerDoctorDTO.Email);
            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdSpecAsync(new DoctorSpecifications(user!.Id));

            if (doctor != null)
            {
                // Doctor exists, check if already linked to this clinic
                var isLinked = await _unitOfWork.Repository<ClinicDoctor>()
                    .GetByIdSpecAsync(new ClinicDoctorSpecifications(clinic.Id, doctor.Id));
                    
                if (isLinked !=null)
                    return BadRequest(new BaseApiResponse(400, "Doctor is already linked to this clinic."));

                var link = new ClinicDoctor
                {
                    ClinicId = clinicId.Value,
                    DoctorId = doctor.Id
                };

                await _unitOfWork.Repository<ClinicDoctor>().AddAsync(link);
                await _unitOfWork.SaveChangeAsync();

                return Ok(new BaseApiResponse(200, "doctor linked to clinic."));
            }

            // Doctor doesn't exist: create new doctor account
            var doctorUser = DoctorMapper.ToApplicationUser(registerDoctorDTO);
            doctorUser.EmailConfirmed = true;
            doctorUser.Doctor.IsApproved = true;
            doctorUser.Doctor.IsProfileCompleted = false;
            doctorUser.Doctor.ClinicDoctors = new List<ClinicDoctor>
                {
                    new ClinicDoctor { ClinicId = clinicId.Value }
                };

            var result = await _userManager.CreateAsync(doctorUser, registerDoctorDTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new BaseApiResponse(500, $"Registration failed: {errors}"));
            }

            await _userManager.AddToRoleAsync(doctorUser, "doctor");

            return Ok(new BaseApiResponse(200, "Doctor created and linked to clinic."));
        }


        [Authorize(Roles = "doctor")]
        [HttpGet("/api/doctors/clinic/{clinicId}/weekly-patients")]
        public async Task<IActionResult> GetBookedPatients(int clinicId, [FromQuery] DateTime date)
        {
            var doctorId = GetAuthenticatedDoctorId();
            if (doctorId == null)
                return Unauthorized("Invalid or Unauthorized Doctor.");

            var appointmnetRepo = (IAppointmentRepository)_unitOfWork.Repository<Appointment>();
            var IsNullAppointment = await appointmnetRepo.GetBookedPatientsForDoctorInClinic(date, doctorId.Value, clinicId);
            if (IsNullAppointment == null)
                return BadRequest(new BaseApiResponse(StatusCodes.Status400BadRequest, "Not Found Appointmnet Booked "));
            return Ok(IsNullAppointment);
        }

        [Authorize(Roles = "doctor")]
        [HttpPut]
        [Route("/api/doctor")]
        public async Task<ActionResult<BaseApiResponse>> UpdateDoctor([FromForm] UpdateDoctorDTO doctorDTO)
        {
            var doctorId = GetAuthenticatedDoctorId();
            if (doctorId == null)
                return Unauthorized(new BaseApiResponse(401, "Invalid or Unauthorized Doctor."));

            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(doctorId.Value);
            if (doctor == null)
                return NotFound(new BaseApiResponse(404, "Doctor not found."));

            DoctorMapper.UpdateDoctorData(doctorDTO, doctor);

            var user = await _userManager.FindByIdAsync(doctor.UserId);
            if (user == null)
                return NotFound(new BaseApiResponse(404, "Associated user not found."));

            user.PhoneNumber = doctorDTO.PhoneNumber;
            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
                return StatusCode(500, new BaseApiResponse(500, "Failed to update user."));

            if (doctorDTO.ProfilePicture != null)
            {
                string folderName = "doctorProfilePictures";
                doctor.ProfilePicture = !string.IsNullOrEmpty(doctor.ProfilePicture)
                    ? DocumentSettings.UpdateFile(doctorDTO.ProfilePicture, folderName, doctor.ProfilePicture)
                    : DocumentSettings.UploadFile(doctorDTO.ProfilePicture, folderName);
            }

            await _unitOfWork.BeginTransactionAsync();

            doctor.IsProfileCompleted = true;
            await _unitOfWork.SaveChangeAsync();

            var isUpdated = await _unitOfWork.CommitAsync();

            if (!isUpdated)
                return StatusCode(500, new BaseApiResponse(500, "Failed to update doctor."));

            return Ok(new BaseApiResponse(200, "Doctor updated successfully."));
        }

         [HttpGet("/api/doctors/patients")]
    public async Task<IActionResult> GetDoctorPatients([FromQuery] PaginationSpecParams paginationParams)
    {
        var doctorId = GetAuthenticatedDoctorId();
        if (doctorId == null)
            return Unauthorized(new BaseApiResponse(401, "Invalid or Unauthorized Doctor."));

        var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(doctorId.Value);
        if (doctor == null)
            return NotFound(new BaseApiResponse(404, "Doctor not found."));

        var spec = new DoctorPatientsSpecification(doctorId.Value, paginationParams, paginationParams.Search);

        var patients = await _unitOfWork.Repository<DoctorPatient>().GetAllWithSpecAsync(spec);
        if (patients == null || !patients.Any())
            return NotFound(new BaseApiResponse(StatusCodes.Status404NotFound, "No patients found for this doctor."));

        var patientDTOs = _mapper.Map<IReadOnlyList<PatientDTO>>(patients.Select(dp => dp.Patient));

        return Ok(new PaginationDTO<PatientDTO>()
        {
            data = patientDTOs,
            PageIndex = paginationParams.PageIndex,
            PageSize = paginationParams.PageSize,
            Count = 0 
        });
    }

        [HttpGet("/api/doctors/{id}/clinics")]
        public async Task<ActionResult<PaginationDTO<ClinicDTO>>> GetDoctorClinics(int id, [FromQuery] PaginationSpecParams paginationParams)
        {

            var doctorclinics = await _unitOfWork.Repository<ClinicDoctor>().GetAllWithSpecAsync(new ClinicDoctorSpecifications(id, paginationParams));
            if (doctorclinics == null || !doctorclinics.Any()) return NotFound("Doctor not found. or Clinics not found ");
            var mappedclinic = _mapper.Map<IReadOnlyList<ClinicDTO>>(doctorclinics.Select(dp => dp.Clinic));
            return Ok(new PaginationDTO<ClinicDTO>() { data = mappedclinic, PageIndex = paginationParams.PageIndex, PageSize = paginationParams.PageSize, Count = 0 });
        }

        [Authorize(Roles = "admin,clinic")]
        [HttpDelete]
        [Route("/api/doctors/{id}")]
        public async Task<ActionResult<BaseApiResponse>> DeleteDoctor(int id)
        {
            if (User.IsInRole("clinic"))
            {
                var clinicId = GetAuthenticatedClinicId();
                if (clinicId == null)
                    return Unauthorized("Invalid or Unauthorized Clinic.");
            }

            var doctorRepo = _unitOfWork.Repository<Doctor>();
            var doctor = await doctorRepo.GetByIdAsync(id);
            if (doctor == null)
                return NotFound(new BaseApiResponse(404, "Doctor not found"));

            var appointmentRepo = _unitOfWork.Repository<Appointment>();
            var reviewRepo = _unitOfWork.Repository<Review>();
            var clinicDoctorRepo = _unitOfWork.Repository<ClinicDoctor>();
            var doctorPatientRepo = _unitOfWork.Repository<DoctorPatient>();

            var appointments = await appointmentRepo.GetAllWithSpecAsync(new AppointmentSpecifications(id, false));
            var reviews = await reviewRepo.GetAllWithSpecAsync(new ReviewSpecifications(id));
            var clinicDoctors = await clinicDoctorRepo.GetAllWithSpecAsync(new ClinicDoctorSpecifications(id, false));

            await _unitOfWork.BeginTransactionAsync();

            if (appointments != null && appointments.Any())
                await appointmentRepo.RemoveRangeAsync(appointments);

            if (reviews != null && reviews.Any())
                await reviewRepo.RemoveRangeAsync(reviews);

            if (clinicDoctors != null && clinicDoctors.Any())
                await clinicDoctorRepo.RemoveRangeAsync(clinicDoctors);

            DocumentSettings.DeleteFile("PatientPictures",doctor.ProfilePicture);
            var user = await _userManager.FindByIdAsync(doctor.UserId);

            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new BaseApiResponse(400, "Failed to delete the user"));
                }
            }
            await _unitOfWork.SaveChangeAsync();
            await _unitOfWork.CommitAsync();
            return Ok(new BaseApiResponse(200, "Doctor, related data, and user deleted successfully"));
        }

        [Authorize(Roles = "doctor")]
        [HttpDelete("/api/doctors/patients/{patientId}")]
        public async Task<ActionResult<BaseApiResponse>> UnassignPatientFromDoctor(int patientId)
        {
            var doctorId = GetAuthenticatedDoctorId();
            if (doctorId == null)
                return Unauthorized(new BaseApiResponse(401, "Unauthorized."));

            var doctorPatient = await _unitOfWork.Repository<DoctorPatient>()
                .GetByIdSpecAsync(new DoctorPatientsSpecification(doctorId.Value , patientId));

            if (doctorPatient == null)
                return NotFound(new BaseApiResponse(404, "Patient is not assigned to this doctor."));
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Repository<DoctorPatient>().DeleteAsync(doctorPatient);
            await _unitOfWork.SaveChangeAsync();
            await _unitOfWork.CommitAsync();

            return Ok(new BaseApiResponse(200, "Patient unassigned from doctor successfully."));
        }

        [Authorize(Roles = "clinic")]
        [HttpPatch("/api/doctors/{id}/block-status")]
        public async Task<ActionResult<BaseApiResponse>> UpdateDoctorApprovalStatus(int id, [FromBody] bool dto)
        {
            var clinicId = GetAuthenticatedClinicId();
            if (clinicId == null)
                return Unauthorized("Invalid or Unauthorized Clinic.");

            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(id);
            if (doctor == null)
                return NotFound(new BaseApiResponse(404, "Doctor not found"));

            doctor.IsApproved = dto;
            doctor.UpdatedDate = DateTime.UtcNow;

            return await SaveChangesAsync(_unitOfWork,
                dto ? "Doctor approved successfully." : "Doctor blocked successfully.",
                "Failed to update doctor status");
        }

        private int? GetAuthenticatedClinicId()
        {
            var clinicIdClaim = User.FindFirst("clinicId");

            if (clinicIdClaim == null || !int.TryParse(clinicIdClaim.Value, out int clinicId))
                return null;

            return clinicId;
        }
        private int? GetAuthenticatedDoctorId()
        {
            var doctorIdClaim = User.FindFirst("doctorId");

            if (doctorIdClaim == null || !int.TryParse(doctorIdClaim.Value, out int doctorId))
                return null;

            return doctorId;
        }

    }
}
