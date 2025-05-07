using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs.Authentication
{
    public class RegisterDTO
    {
        public RegisterPatientDTO? Patient { get; set; }     
        public RegisterClinicDTO? Clinic { get; set; }
    }
}
