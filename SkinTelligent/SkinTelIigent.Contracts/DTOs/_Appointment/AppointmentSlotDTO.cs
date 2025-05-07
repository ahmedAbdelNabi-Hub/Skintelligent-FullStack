using SkinTelIigent.Contracts.DTOs._Doctor;
using SkinTelIigent.Contracts.DTOs.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs._Appointment
{
    public class AppointmentSlotDTO
    {
        public int Id { get; set; }
        public string TimeSlot { get; set; }
        public bool IsBooked { get; set; }
        public DoctorDTO Doctor { get; set; }
        public PatientDTO? Patient { get; set; }
    }
}
