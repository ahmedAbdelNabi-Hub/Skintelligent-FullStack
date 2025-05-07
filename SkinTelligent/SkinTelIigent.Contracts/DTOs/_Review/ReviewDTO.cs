using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs._Review
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public string PatientName { get; set; }
        public string PatientImage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
