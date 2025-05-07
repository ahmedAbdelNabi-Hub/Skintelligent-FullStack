using SkinTelIigent.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Interface
{
    public interface IAnalysisRepository
    {
        Task<DashboardCounters?> GetAdminDashboardOverviewAsync();
        Task<DashboardCounters?> GetDoctorDashboardCountersAsync(int doctorId);
        Task<List<AppointmentVolumeData>> GetCompletedAppointmentsByMonthAsync(int? year = null);
        Task<ClinicDashboard> GetClinicDashboardDataAsync(string clinicId);
        Task<DoctorDashboard> GetDoctorDashboardDataAsync(string doctorId);
        Task<List<DoctorGrowthData>> GetDoctorGrowthOverTimeAsync();
        Task<List<PatientGrowthData>> GetPatientGrowthOverTimeAsync();
        Task<List<AppointmentVolumeData>> GetAppointmentVolumeOverTimeAsync();
    }

}
