using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.DTOs._Review
{
    public class CreateReviewDTO
    {
        public int DoctorId { get; set; }
        public int ClinictId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}
