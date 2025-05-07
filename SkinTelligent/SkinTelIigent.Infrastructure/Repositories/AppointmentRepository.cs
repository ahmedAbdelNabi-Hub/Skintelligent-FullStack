using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Core.Specification;
using SkinTelIigent.Core.Specification.PatientSpecific;
using SkinTelIigent.Infrastructure.Data;

namespace SkinTelIigent.Infrastructure.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>,IAppointmentRepository
    {
        private readonly SkinTelIigentDbContext _dbContext; 
        public AppointmentRepository(SkinTelIigentDbContext dbcontext) : base(dbcontext) 
        {
            _dbContext = dbcontext;
        
        }

        public async Task<bool> AddRangeAsync(List<Appointment> appointments)
        {
            try
            {
                await _dbContext.AddRangeAsync(appointments);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Dictionary<string, List<object>>> GetAppointmentsForWeekAsync(DateTime date, int clinicId, int doctorId)
        {
            var startOfWeek = date.Date.AddDays(-(int)date.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            var appointments = await _dbContext.Appointments
                .Where(a =>
                    a.ClinicId == clinicId &&
                    a.DoctorId == doctorId &&
                    a.StartTime >= startOfWeek &&
                    a.StartTime < endOfWeek)
                .ToListAsync();

            var groupedByDay = Enum.GetValues(typeof(DayOfWeek))
                .Cast<DayOfWeek>()
                .ToDictionary(
                    day => day.ToString(),
                    day => appointments
                        .Where(a => a.StartTime.DayOfWeek == day)
                        .Select(a => new
                        {
                            a.Id,
                            a.Note,
                            a.StartTime,
                            a.EndTime,
                            a.IsCanceled,
                            a.IsBooked,
                            a.RepeatUntil
                        }).Cast<object>()
                        .ToList()
                );

            return groupedByDay;
        }

        public async Task<List<dynamic>> GetBookedPatientsForDoctorInClinic(DateTime date, int doctorId, int clinicId)
        {
            var startOfWeek = date.Date.AddDays(-(int)date.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            var result = await _dbContext.Appointments
                .Where(a =>
                    a.ClinicId == clinicId &&
                    a.DoctorId == doctorId &&
                    a.PatientId != null &&
                    a.StartTime >= startOfWeek &&
                    a.StartTime < endOfWeek)
                .Include(a => a.Patient).ThenInclude(p => p!.User)
                .GroupBy(a => a.RepeatDay)
                .Select(g => new
                {
                    Day = g.Key,
                    Patients = g.Select(a => new
                    {
                        AppointmentId = a.Id,
                        a.Patient!.Id,
                        a.Patient.FirstName,
                        a.Patient.User.Email,
                        PhoneNumber = a.Patient.User.PhoneNumber,
                        a.Patient.LastName,
                        a.StartTime,
                        a.EndTime,
                        a.IsCanceled,
                        a.IsCompleted,
                    }).ToList()
                })
                .ToListAsync();

            return result.Cast<dynamic>().ToList();
        }
    }
}
