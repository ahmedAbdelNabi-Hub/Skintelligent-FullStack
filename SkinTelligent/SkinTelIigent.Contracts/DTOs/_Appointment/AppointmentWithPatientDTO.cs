using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs._Appointment
{
    public class AppointmentWithPatientDTO
    {
        public string? PatientImage { get; set; } = string.Empty;
        public string? PatientName { get; set; }=string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public int Age { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
    }
}
