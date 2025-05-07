using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkinTelIigent.Contracts.DTOs._Appointment;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelIigent.Core.Specification;
using SkinTelIigentContracts.CustomResponses;

namespace SkinTelIigent.Contracts.Interface
{
    public interface IAppointmentService
    {
        Task<CancelAppointmentResponseDTO> CancelByDoctorAsync(int appointmnetId, int doctorId);
        Task<BookAppointmentDTO> BookAsync(int appointmentId, int patientId);
        Task<CancelAppointmentResponseDTO> CancelByPatinetAsync(int appointmentId, int patientId);

        Task ProcessAppointmentsAsync();
    }
}

