using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Core.Models;
using SkinTelIigent.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Infrastructure.Repositories
{
    public class AnalysisRepository : IAnalysisRepository
    {
        private readonly SkinTelIigentDbContext _dbContext;

        public AnalysisRepository(SkinTelIigentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DashboardCounters?> GetAdminDashboardOverviewAsync()
        {
            var result = new DashboardCounters
            {
                TotalDoctors = await _dbContext.Doctors.CountAsync(),
                TotalPatients = await _dbContext.Patients.CountAsync(),
                TotalAppointments = await _dbContext.Appointments.CountAsync(),
                TotalClinics = await _dbContext.Clinic.Where(c => c.IsApproved).CountAsync()
            };

            return result;
        }
        public async Task<List<DoctorGrowthData>> GetDoctorGrowthOverTimeAsync()
        {
            var result = await _dbContext.Doctors
            .AsNoTracking()
            .GroupBy(d => new { d.CreatedDate.Year, d.CreatedDate.Month })
                    .Select(g => new DoctorGrowthData
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        DoctorCount = g.Count()
                    })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToListAsync();

            return result;

        }
        public async Task<List<PatientGrowthData>> GetPatientGrowthOverTimeAsync()
        {
            var result = await _dbContext.Patients
                     .AsNoTracking()
                     .GroupBy(p => new { p.CreatedDate.Year, p.CreatedDate.Month })
                     .Select(g => new PatientGrowthData
                     {
                         Year = g.Key.Year,
                         Month = g.Key.Month,
                         PatientCount = g.Count()
                     })
                     .OrderBy(g => g.Year)
                     .ThenBy(g => g.Month)
                     .ToListAsync();

            return result;

        }
        public async Task<List<AppointmentVolumeData>> GetAppointmentVolumeOverTimeAsync()
        {
            var result = await _dbContext.Appointments
                        .AsNoTracking()
                        .GroupBy(a => new { a.CreatedDate.Year, a.CreatedDate.Month })
                        .Select(g => new AppointmentVolumeData
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            AppointmentCount = g.Count()
                        })
                        .OrderBy(g => g.Year)
                        .ThenBy(g => g.Month)
                        .ToListAsync();

            return result;

        }


        public Task<ClinicDashboard> GetClinicDashboardDataAsync(string clinicId)
        {
            throw new NotImplementedException();
        }

        public Task<DoctorDashboard> GetDoctorDashboardDataAsync(string doctorId)
        {
            throw new NotImplementedException();
        }

        public async Task<DashboardCounters?> GetDoctorDashboardCountersAsync(int doctorId)
        {
            var totalAppointments = await _dbContext.Appointments
                .Where(d=>d.DoctorId == doctorId)
                                   .CountAsync(a => a.IsBooked);

            var totalCompleted = await _dbContext.Appointments
                .Where(d => d.DoctorId == doctorId)
                                 .CountAsync(a => a.IsCompleted);

            var totalCanceled = await _dbContext.Appointments
                .Where(d => d.DoctorId == doctorId)
                               .CountAsync(a => a.IsCanceled);

            var activePatients = await _dbContext.DoctorPatient
                .Where(a => a.DoctorId==doctorId)
                .Select(a => a.PatientId)
                .Distinct()
                .CountAsync();

            var result = new DashboardCounters
            {
                TotalDoctors = null,
                TotalPatients = null,
                TotalAppointments = totalAppointments,
                TotalClinics = null,
                TotalCompletedAppointments = totalCompleted,
                TotalCanceledAppointments = totalCanceled,
                TotalActivePatients = activePatients
            };

            return result;
        }


        public async Task<List<AppointmentVolumeData>> GetCompletedAppointmentsByMonthAsync(int? year = null)
        {
            int targetYear = year ?? DateTime.UtcNow.Year;

            return await _dbContext.Appointments
                .Where(a => a.IsCompleted && a.StartTime.Year == targetYear)
                .GroupBy(a => new { a.StartTime.Year, a.StartTime.Month })
                .Select(g => new AppointmentVolumeData
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    AppointmentCount = g.Count()
                })
                .ToListAsync();
        }



    }
}
