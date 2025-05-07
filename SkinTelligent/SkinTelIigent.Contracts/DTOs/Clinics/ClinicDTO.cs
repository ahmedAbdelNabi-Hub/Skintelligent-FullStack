using SkinTelIigent.Contracts.DTOs._Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs.Clinics
{
    public class ClinicDTO
    {
        public int id { get; set; }     
        public string ClinicName { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }

    }
}
