using SkinTelIigent.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Specification
{
    public class ReviewSpecifications : BaseSpecifications<Review>
    {
        public ReviewSpecifications() { }
        public ReviewSpecifications(int doctorId, PaginationSpecParams paginationParams , bool includePatient = false): base(r => r.DoctorId == doctorId)
        {
            if (includePatient)
                AddInclude(r => r.Patient);

            AddOrderByDescending(r => r.CreatedAt);
            ApplyPagination(paginationParams.PageSize * (paginationParams.PageIndex - 1), paginationParams.PageSize);

        }
        public ReviewSpecifications(int doctorId) : base(r => r.DoctorId == doctorId)
        {
            
        }

        public ReviewSpecifications(int doctorId, int patientId , int clinicId)
            : base(r => r.DoctorId == doctorId && r.PatientId == patientId&& r.ClinicId==clinicId)
        {
        }
     


    }
}
