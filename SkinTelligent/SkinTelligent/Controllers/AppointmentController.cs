using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinTelIigent.Contracts.Interface;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelIigent.Core.Interface;
using SkinTelIigentContracts.CustomResponses;
using SkinTelIigent.Contracts.DTOs._Appointment;
using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Core.Specification;
using SkinTelIigent.Contracts.DTOs;
namespace SkinTelligent.Api.Controllers
{

    public class AppointmentController : BaseController
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationSender _notificationSender;

        public AppointmentController(
            IAppointmentService appointmentService,
            IUnitOfWork unitOfWork,
            INotificationSender notificationSender)
        {
            _appointmentService = appointmentService;
            _unitOfWork = unitOfWork;
            _notificationSender = notificationSender;
        }

        [Authorize(Roles = "doctor")]
        [HttpPost("/api/appointments")]
        public async Task<ActionResult<BaseApiResponse>> CreateAppointment([FromBody] AppointmentRangeDto dto)
        {
            var doctorId = GetAuthenticatedDoctorId();
            if (doctorId == null)
                return Unauthorized("Invalid or Unauthorized doctor.");


            var appointments = new List<Appointment>();
            var startDate = dto.StartFromDate.Date;

            while ((int)startDate.DayOfWeek != dto.Day)
            {
                startDate = startDate.AddDays(1);
            }

            var repeatUntil = dto.IsRepeating ? dto.RepeatUntil ?? startDate : startDate;

            var dailyStartTime = TimeSpan.Parse(dto.DailyStartTime);
            var dailyEndTime = TimeSpan.Parse(dto.DailyEndTime);

            for (var currentDate = startDate; currentDate <= repeatUntil; currentDate = currentDate.AddDays(7))
            {
                var currentTime = dailyStartTime;

                while (currentTime < dailyEndTime)
                {
                    var startDateTime = currentDate.Add(currentTime);
                    var endDateTime = startDateTime.AddMinutes(20);

                    appointments.Add(new Appointment
                    {
                        DoctorId = doctorId.Value,
                        ClinicId = dto.ClinicId,
                        StartTime = startDateTime,
                        EndTime = endDateTime,
                        IsCanceled = false,
                        Note = dto.Note,
                        IsRepeating = dto.IsRepeating,
                        RepeatDay = dto.Day,
                        RepeatUntil = dto.IsRepeating ? dto.RepeatUntil : null
                    });

                    currentTime = currentTime.Add(TimeSpan.FromMinutes(20));
                }
            }

            var appointmnetRepo = (IAppointmentRepository)_unitOfWork.Repository<Appointment>();
            var isSuccess = await appointmnetRepo.AddRangeAsync(appointments);

            if (!isSuccess)
                return StatusCode(500, new BaseApiResponse(500, "Failed to create appointment slots."));

            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(doctorId.Value);
            var clinic = await _unitOfWork.Repository<Clinic>().GetByIdAsync(dto.ClinicId);

            if (clinic?.UserId != null && doctor != null)
            {
                string title = "New Appointment Slots Added";
                string message = $"Dr. {doctor.FirstName} {doctor.LastName} has created new appointment slots starting from {startDate:yyyy-MM-dd}.";

                await _notificationSender.CreateAndSendAsync(
                    userId: clinic.UserId,
                    message: message,
                    title: title,
                    type: "Appointment");
            }

            return Ok(new BaseApiResponse(200, "Appointment slots created successfully."));
        }



        [Authorize(Roles = "doctor,patient")]
        [HttpGet("/api/appointments/week")]
        public async Task<IActionResult> GetAppointmentsForWeek([FromQuery] DateTime date, [FromQuery] int clinicId, [FromQuery] int? doctorId)
        {
            int finalDoctorId;

            if (User.IsInRole("doctor"))
            {
                var authenticatedDoctorId = GetAuthenticatedDoctorId();
                if (authenticatedDoctorId == null)
                    return Unauthorized("Invalid or Unauthorized doctor.");

                finalDoctorId = authenticatedDoctorId.Value;
            }
            else if (User.IsInRole("patient"))
            {
                if (doctorId == null)
                    return BadRequest("DoctorId is required for patients.");
                finalDoctorId = doctorId.Value;
            }
            else
            {
                return Forbid();
            }

            var appointmnetRepo = (IAppointmentRepository)_unitOfWork.Repository<Appointment>();
            var groupedByDay = await appointmnetRepo.GetAppointmentsForWeekAsync(date, clinicId, finalDoctorId);

            return Ok(groupedByDay);
        }

        [Authorize(Roles = "patient")]
        [HttpPost("/api/appointments/book")]
        public async Task<ActionResult<BaseApiResponse>> BookAppointment(int appointmentId)
        {
            var patientId = GetAuthenticatedPatientId();
            if (patientId == null)
                return Unauthorized(new BaseApiResponse(401, "Invalid or unauthorized patient."));

            var result = await _appointmentService.BookAsync(appointmentId, patientId.Value);

            if (result.statusCode != 200)
                return StatusCode(result.statusCode, new BaseApiResponse(result.statusCode, result.message));

            await _notificationSender.SendAsync(result.DoctorUserId,
                "A new appointment has been booked by a patient.",
                result.DoctorNotificationCount);

            await _notificationSender.SendAsync(result.ClinicUserId,
                "A new appointment has been booked at your clinic.",
                result.ClinicNotificationCount);

            return Ok(new BaseApiResponse(200, result.message));
        }

 
        [Authorize(Roles = "doctor")]
        [HttpDelete("/api/appointments/{Id}")]
        public async Task<ActionResult<BaseApiResponse>> DeleteAppointment(int Id)
        {
            var doctorId = GetAuthenticatedDoctorId();
            if (doctorId == null)
                return Unauthorized("Invalid or Unauthorized doctor.");

            var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(Id);

            if (appointment == null)
                return NotFound(new BaseApiResponse(StatusCodes.Status404NotFound, "Appointment not found"));

            if (appointment.DoctorId != doctorId)
                return Unauthorized("Invalid or Unauthorized To Delete This Appointment.");
            

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Repository<Appointment>().DeleteAsync(appointment);
            await _unitOfWork.SaveChangeAsync();
            await _unitOfWork.CommitAsync(); 
            return Ok(new BaseApiResponse(StatusCodes.Status200OK, "Appointment deleted successfully"));
        }

        [Authorize(Roles = "doctor")]
        [HttpPost("/api/appointments/date")]
        public async Task<IActionResult> GetAppointmentsByDate([FromQuery] int clinicId,[FromQuery] DateTime date,[FromQuery] PaginationSpecParams paginationParams)
        {
            var doctorId = GetAuthenticatedDoctorId();
            if (doctorId == null)
                return Unauthorized("Invalid or Unauthorized doctor.");

            var spec = new AppointmentSpecifications(doctorId.Value, clinicId, date, paginationParams);
            var totalCount = await _unitOfWork
                .Repository<Appointment>().CountWithSpec(spec);

            var appointments = await _unitOfWork
                .Repository<Appointment>()
                .GetProjectedAsync(
                    a => new AppointmentWithPatientDTO
                    {
                        PatientName = a.Patient!.FirstName + " " + a.Patient.LastName,
                        Phone = a.Patient!.Phone,
                        PatientImage = a.Patient.ProfilePicture,
                        Age = EF.Functions.DateDiffYear(a.Patient.DateOfBirth, date),
                        TimeStart = a.StartTime,
                        TimeEnd = a.EndTime,
                    },
                    spec
                );

            var result = new PaginationDTO<AppointmentWithPatientDTO>
            {
                PageIndex = paginationParams.PageIndex,
                PageSize = paginationParams.PageSize,
                Count = totalCount,
                data = appointments
            };

            return Ok(result);
        }


        [Authorize(Roles = "doctor")]
        [HttpPost("/api/appointments/{id}/cancel")]
        public async Task<ActionResult<BaseApiResponse>> CancelAppointment(int id)
        {
            var doctorId = GetAuthenticatedDoctorId();
            if (doctorId == null)
                return Unauthorized("Invalid or unauthorized doctor.");

            var result = await _appointmentService.CancelByDoctorAsync(id, doctorId.Value);

            if (result.statusCode != StatusCodes.Status200OK)
                return StatusCode(result.statusCode, new BaseApiResponse(result.statusCode, result.message));

            await _notificationSender.SendAsync(result.ClinicUserId,
                "An appointment has been cancelled by the doctor",
                result.ClinicNotificationCount!.Value);

           
            return Ok(StatusCode(result.statusCode, new BaseApiResponse(result.statusCode, result.message)));
        }
        


        private int? GetAuthenticatedPatientId()
        {
            var patientIdClaim = User.FindFirst("patientId");

            if (patientIdClaim == null || !int.TryParse(patientIdClaim.Value, out int patientId))
                return null;

            return patientId;
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

