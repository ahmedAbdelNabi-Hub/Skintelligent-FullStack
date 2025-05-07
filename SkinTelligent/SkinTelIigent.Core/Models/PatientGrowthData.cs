using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Models
{
    [NotMapped]
    public class PatientGrowthData
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int PatientCount { get; set; }
    }

}
