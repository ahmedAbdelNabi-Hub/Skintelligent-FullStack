using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs._Appointment
{
    public class AppointmentRangeDto
    {
        public int ClinicId { get; set; }
        public string DailyStartTime { get; set; }  
        public string DailyEndTime { get; set; }    
        public string Note { get; set; }
        public int Day { get; set; }
        public bool IsRepeating { get; set; }
        public DateTime StartFromDate { get; set; }
        public DateTime? RepeatUntil { get; set; }

    
         
    }

}
