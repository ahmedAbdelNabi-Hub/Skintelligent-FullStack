using SkinTelIigent.Contracts.DTOs._Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs.Clinics
{
    public class ClinicWithDoctorDTO : ClinicDTO    
    {
        public List<string> Emails { get; set; }= new List<string>();   

        public int Count { get; set; }
    }
}
