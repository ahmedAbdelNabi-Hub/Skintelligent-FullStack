using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Models
{
    [NotMapped]
    public class DashboardCounters
    {
        public int? TotalDoctors { get; set; }
        public int? TotalPatients { get; set; }
        public int? TotalAppointments { get; set; }
        public int? TotalClinics { get; set; }

        public int? TotalCompletedAppointments { get; set; }
        public int? TotalCanceledAppointments { get; set; }
        public int? TotalActivePatients { get; set; }

        public int? OverallBookings => TotalAppointments; 

    }
}
