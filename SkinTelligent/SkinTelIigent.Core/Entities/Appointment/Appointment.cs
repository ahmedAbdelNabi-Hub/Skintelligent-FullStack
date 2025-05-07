using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkinTelIigent.Core.Entities;

namespace SkinTelIigent.Core.Entities.Appointment
{
    public class Appointment : BaseEntity
    {
        public int? DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int? PatientId { get; set; } 
        public Patient? Patient { get; set; } 
        public int? ClinicId { get; set; }
        public Clinic Clinic { get; set; }
        public string? Note { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsRepeating { get; set; }
        public bool IsBooked { get; set; } = false;
        public bool IsCompleted { get; set; } = false;
        public int? RepeatDay { get; set; } 
        public DateTime? RepeatUntil { get; set; }

        public bool IsDeletedByPatient { get; set; } = false; 
        public DateTime? PatientDeletedAt { get; set; }  
        
        public DateTime CreatedDate { get; set; }
    }
}
