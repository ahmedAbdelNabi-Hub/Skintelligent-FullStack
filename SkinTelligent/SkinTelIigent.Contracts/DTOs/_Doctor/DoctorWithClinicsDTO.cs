using SkinTelIigent.Contracts.DTOs.Clinics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs._Doctor
{
    public class DoctorWithClinicsDTO : DoctorDTO
    {
        public List<ClinicDTO> Clinics { get; set; }
    }
}
