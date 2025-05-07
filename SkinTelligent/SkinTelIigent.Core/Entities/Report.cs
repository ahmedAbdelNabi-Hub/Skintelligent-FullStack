using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Entities
{
    public class Report : BaseEntity
    {
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }
        public string ReportTitle { get; set; }
        public DateTime ReportDate { get; set; }
        public string DiagnosisSuggestions { get; set; }
        public string TreatmentOptions { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public Patient Patient { get; set; }
    }
}
