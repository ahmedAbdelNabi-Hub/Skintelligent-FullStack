using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using SkinTelIigent.Contracts.DTOs._Appointment;
using SkinTelIigent.Contracts.Interface;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Core.Specification;
using SkinTelIigent.Core.Specification.PatientSpecific;
using SkinTelIigentContracts.CustomResponses;

namespace SkinTelIigent.Services
{
    public class AppointmentService : IAppointmentService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;



        public AppointmentService(IAppointmentRepository appointmentRepository,IUnitOfWork unitOfWork,IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<BookAppointmentDTO> BookAsync(int appointmentId, int patientId)
        {

            #region Validate Data
            var appointment = await ValidateAppointment(appointmentId);
            if (appointment is null)
            {
                return new BookAppointmentDTO
                {
                    statusCode = 404,
                    message = "This appointment is either canceled or already booked."
                };
            }

            if (DateTime.UtcNow > appointment.StartTime)
                return new BookAppointmentDTO() { statusCode = StatusCodes.Status400BadRequest, message = "Appointment cannot be booked. It is no longer available." };

            var patient = await ValidatePatient(patientId);
            if (patient is null)
            {
                return new BookAppointmentDTO
                {
                    statusCode = 404,
                    message = "Patient not found."
                };
            }
            await _unitOfWork.BeginTransactionAsync();
            #endregion

            #region  Assign and Track
            await AssignPatientAndTrackDoctorPatient(appointment, patientId);
            #endregion

            #region Notifications
            var doctorUserId = await NotifyDoctorOfBooking(appointment.DoctorId!.Value, patient);
            var clinicUserId = await NotifyClinicOfBooking(appointment.ClinicId!.Value, appointment.DoctorId!.Value, patient);
            await _unitOfWork.SaveChangeAsync();
            await _unitOfWork.CommitAsync();
            #endregion

            #region Count Notifications
            int clinicNotificationCount = 0;
            int doctorNotificationCount = 0;
            if (!string.IsNullOrEmpty(doctorUserId))
            {
                doctorNotificationCount = await GetUnreadNotificationCount(doctorUserId);
            }
            if (!string.IsNullOrEmpty(clinicUserId))
            {
                clinicNotificationCount = await GetUnreadNotificationCount(clinicUserId);
            }
            #endregion

            return new BookAppointmentDTO
            {
                DoctorUserId = doctorUserId,
                ClinicUserId = clinicUserId,
                DoctorNotificationCount = doctorNotificationCount,
                ClinicNotificationCount = clinicNotificationCount,
                statusCode = 200,
                message = "Appointment booked successfully."
            };
        }
        public async Task<CancelAppointmentResponseDTO> CancelByDoctorAsync(int appointmentId, int doctorId)
        {
            #region Validate Doctor
            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(doctorId);
            if (doctor is null)
                return new CancelAppointmentResponseDTO(StatusCodes.Status404NotFound, "Doctor not found");
            #endregion

            #region Validate Appointment
            var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(appointmentId);
            if (appointment is null)
                return new CancelAppointmentResponseDTO(StatusCodes.Status404NotFound, "Appointment not found");

            if (appointment.DoctorId != doctorId)
                return new CancelAppointmentResponseDTO(StatusCodes.Status401Unauthorized, "Unauthorized to cancel this appointment");

            if (DateTime.UtcNow > appointment.StartTime)
                return new CancelAppointmentResponseDTO(StatusCodes.Status400BadRequest, "Cannot cancel past appointments.");
            
            if(appointment.IsCanceled ==  true)
                return new CancelAppointmentResponseDTO(StatusCodes.Status400BadRequest, "The appointment is actually canceled.");

            #endregion

            #region Begin Transaction
            await _unitOfWork.BeginTransactionAsync();
            #endregion

            #region Cleanup Doctor-Patient Relation
            Patient? patient = null;
            var patientId = appointment.PatientId;
            if (patientId.HasValue && appointment.IsBooked)
            {
                patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(patientId.Value);
                if (patient?.LastVisitDate == null)
                {
                    var doctorPatient = await _unitOfWork.Repository<DoctorPatient>()
                        .GetByIdSpecAsync(new DoctorPatientsSpecification(doctorId, patientId.Value));

                    if (doctorPatient != null)
                        await _unitOfWork.Repository<DoctorPatient>().DeleteAsync(doctorPatient);
                }
            }
            #endregion

            #region Cancel Appointment
            appointment.IsCanceled = true;
            await _unitOfWork.SaveChangeAsync();
            #endregion

            #region Notify Patient via Email
            if (patientId.HasValue && patient == null)
            {
                patient = await _unitOfWork.Repository<Patient>()
                    .GetByIdSpecAsync(new PatientSpecification(patientId.Value));
            }

            if (patient?.User?.Email != null)
            {
                var emailSent = await _emailService.SendEmailAsync(
                    patient.User.Email,
                    "Appointment Cancelled",
                    "Your appointment has been cancelled by the doctor.");

                if (!emailSent)
                {
                    await _unitOfWork.RollbackAsync();
                    return new CancelAppointmentResponseDTO(StatusCodes.Status500InternalServerError,
                        "Appointment cancelled, but failed to notify the patient via email.");
                }
            }
            #endregion

            #region add Notify Clinic 
            var clinicUserId = await NotifyClinicOfCancellation(appointment.ClinicId!.Value, appointment.DoctorId!.Value, patient!,"doctor");
            #endregion

            #region Commit
            await _unitOfWork.SaveChangeAsync();
            await _unitOfWork.CommitAsync();
            #endregion

            #region  count Notification
            int clinicNotificationCount = 0;
            if (!string.IsNullOrEmpty(clinicUserId))
            {
                clinicNotificationCount = await GetUnreadNotificationCount(clinicUserId);
            }
            #endregion

            #region Return Success (Clinic hub to be handled in controller)
            return new CancelAppointmentResponseDTO(StatusCodes.Status200OK, "Appointment cancelled successfully")
            {
                ClinicUserId = clinicUserId ,
                ClinicNotificationCount=clinicNotificationCount
            };
            #endregion
        }

        public async Task<CancelAppointmentResponseDTO> CancelByPatinetAsync(int appointmentId, int patientId)
        {
            #region Validate Appointment
            var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(appointmentId);
            if (appointment == null)
                return new CancelAppointmentResponseDTO(404, "Appointment not found");

            if (appointment.PatientId != patientId)
                return new CancelAppointmentResponseDTO(StatusCodes.Status403Forbidden, "You are not allowed to cancel this appointment.");

            if (DateTime.UtcNow >= appointment.StartTime)
                return new CancelAppointmentResponseDTO(400, "You can only cancel before the appointment starts.");

            if (appointment.IsCanceled)
                return new CancelAppointmentResponseDTO(400, "The appointment is already cancelled.");
            #endregion

            #region Begin Transaction
            await _unitOfWork.BeginTransactionAsync();
            #endregion

            #region Cancel Booking
            appointment.PatientId = null;
            appointment.IsBooked = false;
            appointment.IsDeletedByPatient = false;
            appointment.PatientDeletedAt = null;
            appointment.IsCanceled = true;
            await _unitOfWork.SaveChangeAsync();
            #endregion

            #region Load Patient Info
            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(patientId);
            #endregion

            #region Notify Doctor
            var doctorUserId = await NotifyDoctorOfCancellation(appointment.DoctorId!.Value, patient!);
            #endregion

            #region Notify Clinic
            var clinicUserId = await NotifyClinicOfCancellation(appointment.ClinicId!.Value, appointment.DoctorId!.Value, patient!, "patient");
            #endregion

            #region Commit Transaction
            await _unitOfWork.SaveChangeAsync();
            await _unitOfWork.CommitAsync();
            #endregion

            #region Notification Count
            int clinicNotificationCount = 0;
            if (!string.IsNullOrEmpty(clinicUserId))
                clinicNotificationCount = await GetUnreadNotificationCount(clinicUserId);

            int doctorNotificationCount = 0;
            if (!string.IsNullOrEmpty(doctorUserId))
                doctorNotificationCount = await GetUnreadNotificationCount(doctorUserId);
            #endregion

            #region Return
            return new CancelAppointmentResponseDTO(200, "Appointment unbooked successfully")
            {
                ClinicUserId = clinicUserId,
                DoctorUserId = doctorUserId,
                DoctorNotificationCount = doctorNotificationCount,
                ClinicNotificationCount = clinicNotificationCount
            };
            #endregion
        }


        public async Task ProcessAppointmentsAsync()
        {
            var now = DateTime.UtcNow;
            Console.WriteLine($"[Hangfire Job Executed] Time: {now}");

            var spec = new AppointmentSpecifications().GetPastBookedAppointmentsNotCompleted(now);
            var appointments = await _unitOfWork.Repository<Appointment>().GetAllWithSpecAsync(spec, true);

            if (appointments == null || !appointments.Any())
            {
                Console.WriteLine("No appointments to process at: " + now);
                return;
            }

            await _unitOfWork.BeginTransactionAsync();

            foreach (var appointment in appointments)
            {
                appointment.IsCompleted = true;

                if (appointment.Patient != null)
                {
                    appointment.Patient.LastVisitDate = appointment.StartTime;
                    Console.WriteLine(":IsCompleted " + now);
                }
            }

            await _unitOfWork.SaveChangeAsync();
            await _unitOfWork.CommitAsync();
        }

 
        #region Validation
        private async Task<Appointment?> ValidateAppointment(int appointmentId)
        {
            var appointment = await _unitOfWork.Repository<Appointment>()
                .GetByIdSpecAsync(new AppointmentSpecifications(appointmentId));

            if (appointment == null || appointment.IsCanceled)
                return null;

            if (appointment.IsBooked && appointment.PatientId != null)
                return null;

            return appointment;
        }

        private async Task<Patient> ValidatePatient(int patientId)
        {
            return await _unitOfWork.Repository<Patient>().GetByIdAsync(patientId);
        }
        private async Task<Doctor> ValidateDoctor(int doctorId)
        {
            return await _unitOfWork.Repository<Doctor>().GetByIdAsync(doctorId);
        }
        private async Task<Clinic> ValidateClinic(int clinicId)
        {
            return await _unitOfWork.Repository<Clinic>().GetByIdAsync(clinicId);
        }
        #endregion

        #region Assign Patient and Doctor Link
        private async Task AssignPatientAndTrackDoctorPatient(Appointment appointment, int patientId)
        {
            appointment.PatientId = patientId;
            appointment.IsBooked = true;

            var doctorPatientRepo = _unitOfWork.Repository<DoctorPatient>();
            var doctorPatientLink = await doctorPatientRepo
                .GetByIdSpecAsync(new DoctorPatientsSpecification(appointment.DoctorId!.Value, patientId));

            if (doctorPatientLink == null)
            {
                await doctorPatientRepo.AddAsync(new DoctorPatient
                {
                    DoctorId = appointment.DoctorId!.Value,
                    PatientId = patientId
                });
            }
        }
        #endregion

        #region Notifications
        private async Task<string?> NotifyDoctorOfBooking(int doctorId, Patient patient)
        {
            var doctorUserId = await DoctorUserId(doctorId);
            var message = $"New Appointment Booked\nPatient: {patient.FirstName} {patient.LastName} : Phone: {patient.Phone}";

            return await SendNotificationAsync(doctorUserId, "New Appointment Booked", message);
        }

        private async Task<string?> NotifyClinicOfBooking(int clinicId, int doctorId, Patient patient)
        {
            var clinicUserId = await ClinicUserId(clinicId);
            var doctor = await ValidateDoctor(doctorId);

            var message = $"Patient {patient.FirstName} {patient.LastName} has booked an appointment with Dr. {doctor.FirstName} {doctor.LastName} at your clinic.\n" +
                          $"Contact: {patient.Phone}";

            return await SendNotificationAsync(clinicUserId, "New Appointment Booked", message);
        }

        private async Task<string?> NotifyClinicOfCancellation(int clinicId, int doctorId, Patient patient, string cancelledBy)
        {
            var clinicUserId = await ClinicUserId(clinicId);
            var doctor = await ValidateDoctor(doctorId);

            string title = "Appointment Cancelled";
            string message;

            if (cancelledBy.ToLower() == "patient")
            {
                message = $"Patient {patient.FirstName} {patient.LastName} has cancelled their appointment with Dr. {doctor.FirstName} {doctor.LastName}.\n" +
                          $"Contact Info: {patient.Phone}";
                title = "Appointment Cancelled by Patient";
            }
            else if (cancelledBy.ToLower() == "doctor")
            {
                message = $"Dr. {doctor.FirstName} {doctor.LastName} has cancelled their appointment with Patient {patient.FirstName} {patient.LastName}.\n" +
                          $"Contact Info: {patient.Phone}";
                title = "Appointment Cancelled by Doctor";
            }
            else
            {
                message = $"The appointment between Dr. {doctor.FirstName} {doctor.LastName} and Patient {patient.FirstName} {patient.LastName} has been cancelled.\n" +
                          $"Contact Info: {patient.Phone}";
            }

            return await SendNotificationAsync(clinicUserId, title, message);
        }


        private async Task<string?> NotifyDoctorOfCancellation(int doctorId, Patient patient)
        {
            var doctorUserId = await DoctorUserId(doctorId);
            var message = $"The appointment with Patient {patient.FirstName} {patient.LastName} has been canceled.\n" +
                          $"Contact: {patient.Phone}";

            return await SendNotificationAsync(doctorUserId, "Appointment Canceled", message);
        }

        private async Task<string?> ClinicUserId(int clinicId)
        {
            return await _unitOfWork.Repository<Clinic>()
                 .GetQueryableWithSpec(new ClinicSpecifications(clinicId))
                 .Select(c => c.UserId)
                 .FirstOrDefaultAsync();
        }
        private async Task<string?> DoctorUserId(int doctorId)
        {
            return await _unitOfWork.Repository<Doctor>()
                .GetQueryableWithSpec(new DoctorSpecifications(doctorId))
                .Select(d => d.UserId)
                .FirstOrDefaultAsync();
        }
        private async Task<string?> SendNotificationAsync(string? userId, string title, string message, string type = "appointment")
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            var notification = new Notification
            {
                Title = title,
                Message = message,
                UserId = userId,
                Type = type
            };

            await _unitOfWork.Repository<Notification>().AddAsync(notification);
            return userId;
        }


        #endregion

        #region Notification count
        private async Task<int> GetUnreadNotificationCount(string userId)
        {
            return await _unitOfWork.Repository<Notification>().CountWithSpec(new NotificationSpecification(userId));
                
        }

        #endregion

    }
}

