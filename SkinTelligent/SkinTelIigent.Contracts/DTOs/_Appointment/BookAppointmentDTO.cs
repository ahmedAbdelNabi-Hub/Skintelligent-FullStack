using SkinTelIigentContracts.CustomResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs._Appointment
{
    public class BookAppointmentDTO : BaseApiResponse
    {
        public string? DoctorUserId { get; set; }
        public string? ClinicUserId { get; set; }

        public int DoctorNotificationCount { get; set; }
        public int ClinicNotificationCount { get; set; }

    }
}
