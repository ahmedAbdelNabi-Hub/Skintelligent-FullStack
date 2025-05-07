using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


     namespace SkinTelIigent.Core.Entities
     {
        public class Review :BaseEntity
        {

             public int DoctorId { get; set; }
             public Doctor Doctor { get; set; }

             public int PatientId { get; set; }
             public Patient Patient { get; set; }

             public int? ClinicId { get; set; } 
             public Clinic Clinic { get; set; }

             public string Comment { get; set; }
             public int Rating { get; set; }
             public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
}
