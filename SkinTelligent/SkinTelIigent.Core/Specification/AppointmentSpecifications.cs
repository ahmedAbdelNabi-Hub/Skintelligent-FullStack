using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkinTelIigent.Core.Entities.Appointment;

namespace SkinTelIigent.Core.Specification
{
    public class AppointmentSpecifications : BaseSpecifications<Appointment>
    {
        public AppointmentSpecifications()
        {
            
        }
        public AppointmentSpecifications(int doctorId , bool IsInclude = false) : base(a=>a.DoctorId==doctorId)
        {
            
        }
        public AppointmentSpecifications(int doctorId, int clinicId , DateTime date, PaginationSpecParams paginationParams)
        {
            AddInclude(a => a.Patient!);
            AddCriteria(a =>
            a.DoctorId == doctorId &&
            a.ClinicId == clinicId &&
            a.IsBooked &&
            a.PatientId != null &&
            a.StartTime.Date == date.Date);
            ApplyPagination(paginationParams.PageSize * (paginationParams.PageIndex - 1), paginationParams.PageSize);

        }
        public AppointmentSpecifications(int appointmentId) : base(a=>a.Id==appointmentId)
        {
            AddInclude(a => a.Doctor);
        }
        public AppointmentSpecifications(int id, int doctorId) : base(a => a.Id == id && a.DoctorId == doctorId)
        {

        }

        public AppointmentSpecifications GetClinicAppointments(int clinicId, DateTime date)
        {
            var startOfToday = date.Date; 
            var endOfToday = date.Date.AddDays(1).AddMilliseconds(-1); 
            AddCriteria(a => a.ClinicId == clinicId
                             && !a.IsCanceled
                             && a.PatientId != null
                             && a.StartTime >= startOfToday
                             && a.StartTime <= endOfToday);

            AddInclude(a => a.Doctor);
            AddInclude(a => a.Patient!);
            AddOrderBy(a => a.StartTime);

            return this;
        }
        public AppointmentSpecifications GetBookedAppointmentsByPatientId(int patientId)
        {
            AddCriteria(a =>
                a.PatientId == patientId &&
                a.IsBooked &&
                !a.IsDeletedByPatient);
            AddInclude(a=>a.Doctor);
            AddInclude(a => a.Clinic) ;

            return this;    
        }
        public AppointmentSpecifications GetPastBookedAppointmentsNotCompleted(DateTime nowUtc)
        {
            AddCriteria(a =>
                a.IsBooked &&
                !a.IsCanceled &&
                !a.IsCompleted &&
                a.EndTime <= nowUtc);

            AddInclude(a => a.Patient!);
            return this;
        }


    }
}
