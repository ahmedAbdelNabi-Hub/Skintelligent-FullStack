using SkinTelIigent.Contracts.DTOs.Clinics;
using SkinTelIigent.Contracts.DTOs.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs._Doctor
{
    public class DoctorWithPatientsDTO
    {
    
        public List<PatientDTO> Patients { get; set; } = new();

    }
}
