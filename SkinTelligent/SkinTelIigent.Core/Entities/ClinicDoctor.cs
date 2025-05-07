using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Entities
{
    public class ClinicDoctor : BaseEntity
    {
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }

        public int ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }
    }

}
