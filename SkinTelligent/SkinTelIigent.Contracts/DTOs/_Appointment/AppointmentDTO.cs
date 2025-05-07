using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkinTelIigent.Core.Entities.Appointment;

namespace SkinTelIigent.Contracts.DTOs._Appointment
{
    public class AppointmentDTO
    {

        public int Id { get; set; }
        public string? Note { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCanceled { get; set; }
        public string? DoctorName { get; set; }
        public string? ClinicName { get; set; }
        public string? ClinicAddress { get; set; }
    }
}
