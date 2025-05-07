using SkinTelIigent.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs.Analysis
{
    public class AdminDashboardFullDTO
    {
        public DashboardCounters? Overview { get; set; }
        public List<DoctorGrowthData> DoctorGrowth { get; set; } = new();
        public List<PatientGrowthData> PatientGrowth { get; set; } = new();
        public List<AppointmentVolumeData> AppointmentVolume { get; set; } = new();
    }
}
