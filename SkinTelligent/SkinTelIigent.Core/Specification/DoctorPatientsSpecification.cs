using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkinTelIigent.Core.Specification
{
    public class DoctorPatientsSpecification : BaseSpecifications<DoctorPatient>
    {
        public DoctorPatientsSpecification()
        {
        }

        public DoctorPatientsSpecification(int doctorId, PaginationSpecParams paginationParams, string? search = null)
            : base(dp => dp.DoctorId == doctorId)
        {
            AddInclude(dp => dp.Patient);

            // Add search criteria if search is provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                AddCriteria(dp =>
                    dp.Patient.FirstName.ToLower().Contains(search.ToLower()) ||
                    dp.Patient.LastName.ToLower().Contains(search.ToLower()));
            }

            ApplyPagination(paginationParams.PageSize * (paginationParams.PageIndex - 1), paginationParams.PageSize);
        }

        public DoctorPatientsSpecification(int doctorId)
            : base(dp => dp.DoctorId == doctorId)
        {
            AddInclude(dp => dp.Patient);
        }

        public DoctorPatientsSpecification(List<int> doctorIds)
            : base(dp => doctorIds.Contains(dp.DoctorId))
        {
            AddInclude(dp => dp.Doctor);
            AddInclude(dp => dp.Patient);
        }

        public DoctorPatientsSpecification(int doctorId, int patientId)
            : base(dp => dp.DoctorId == doctorId && dp.PatientId == patientId)
        {
        }

        public DoctorPatientsSpecification GetPatientDoctors(int patientId, PaginationSpecParams paginationParams)
        {
            AddCriteria(dp => dp.PatientId == patientId);
            AddInclude(dp => dp.Doctor);
            ApplyPagination(paginationParams.PageSize * (paginationParams.PageIndex - 1), paginationParams.PageSize);
            return this;
        }
    }
}
