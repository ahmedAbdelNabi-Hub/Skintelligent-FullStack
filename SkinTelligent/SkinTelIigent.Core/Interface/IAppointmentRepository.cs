using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelIigent.Core.Specification;

namespace SkinTelIigent.Core.Interface
{
    public interface IAppointmentRepository
    {
        Task<bool> AddRangeAsync(List<Appointment> appointments);
        Task<Dictionary<string, List<object>>> GetAppointmentsForWeekAsync(DateTime date, int clinicId, int doctorId);
        Task<List<object>> GetBookedPatientsForDoctorInClinic(DateTime date, int doctorId, int clinicId);

    }


}
