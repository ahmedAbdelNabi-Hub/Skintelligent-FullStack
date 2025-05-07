using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs._Appointment
{
    public class CancelAppointmentDTO
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int AppointmentId { get; set; }
    }
}
