using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Specification
{
    public class ClinicDoctorSpecifications : BaseSpecifications<ClinicDoctor>
    {
        public ClinicDoctorSpecifications()
        {
            
        }

        public ClinicDoctorSpecifications(int doctorId , bool IsInclude=false):base(cd=>cd.DoctorId==doctorId)
        {
            
        }

        public ClinicDoctorSpecifications(PaginationSpecParams paginationParams)
        {
            AddInclude(cd => cd.Clinic);
            AddInclude(cd => cd.Doctor);

            ApplyPagination(paginationParams.PageSize * (paginationParams.PageIndex - 1), paginationParams.PageSize);

        }
        public ClinicDoctorSpecifications(int clinicId, DoctorFilterParams filterParams) 
        {
            AddInclude(cd => cd.Doctor);

            Expression<Func<ClinicDoctor, bool>> predicate = cd => cd.ClinicId == clinicId;

            if (!string.IsNullOrWhiteSpace(filterParams.Filter))
            {
                var filter = filterParams.Filter.Trim().ToLower();
                switch (filter)
                {
                    case "completed":
                        predicate = predicate.And(cd => cd.Doctor.IsProfileCompleted);
                        break;
                    case "not_completed":
                        predicate = predicate.And(cd => !cd.Doctor.IsProfileCompleted);
                        break;
                    case "active":
                        predicate = predicate.And(cd => cd.Doctor.IsApproved);
                        break;
                    case "blocked":
                        predicate = predicate.And(cd => !cd.Doctor.IsApproved);
                        break;
                    case "highest_rating":
                        AddOrderByDescending(cd => cd.Doctor.Reviews.Any()
                            ? cd.Doctor.Reviews.Average(r => r.Rating)
                            : 0);
                        break;
                    case "lowest_rating":
                        AddOrderBy(cd => cd.Doctor.Reviews.Any()
                            ? cd.Doctor.Reviews.Average(r => r.Rating)
                            : 0);
                        break;
                }
            }

            // Combine search
            if (!string.IsNullOrWhiteSpace(filterParams.Search))
            {
                var search = filterParams.Search.ToLower();
                predicate = predicate.And(cd =>
                    cd.Doctor.FirstName.ToLower().Contains(search) ||
                    cd.Doctor.LastName.ToLower().Contains(search) ||
                    cd.Doctor.User.Email!.ToLower().Contains(search) ||
                    cd.Doctor.User.PhoneNumber!.Contains(search));
            }

            AddCriteria(predicate);



        }
        public ClinicDoctorSpecifications(int doctorId, PaginationSpecParams paginationParams)
         : base(dp => dp.DoctorId == doctorId) 
        {
            AddInclude(dp => dp.Clinic); 
            ApplyPagination(paginationParams.PageSize * (paginationParams.PageIndex - 1), paginationParams.PageSize);
        }

        public ClinicDoctorSpecifications(int clinicId , int doctorId)
        {
            AddCriteria(cd=>cd.ClinicId==clinicId &&  cd.DoctorId==doctorId);
        }

        public ClinicDoctorSpecifications GetDoctorsByClinicId(int clinicId, DoctorFilterParams filterParams)
          {
                AddInclude(cd => cd.Doctor);
                AddInclude(cd => cd.Doctor.Reviews);


            Expression<Func<ClinicDoctor, bool>> predicate = cd => cd.ClinicId == clinicId;

            if (!string.IsNullOrWhiteSpace(filterParams.Filter))
            {
                var filter = filterParams.Filter.Trim().ToLower();
                switch (filter)
                {
                    case "completed":
                        predicate = predicate.And(cd => cd.Doctor.IsProfileCompleted);
                        break;
                    case "not_completed":
                        predicate = predicate.And(cd => !cd.Doctor.IsProfileCompleted);
                        break;
                    case "active":
                        predicate = predicate.And(cd => cd.Doctor.IsApproved);
                        break;
                    case "blocked":
                        predicate = predicate.And(cd => !cd.Doctor.IsApproved);
                        break;
                    case "highest_rating":
                        AddOrderByDescending(cd => cd.Doctor.Reviews.Any()
                            ? cd.Doctor.Reviews.Average(r => r.Rating)
                            : 0);
                        break;
                    case "lowest_rating":
                        AddOrderBy(cd => cd.Doctor.Reviews.Any()
                            ? cd.Doctor.Reviews.Average(r => r.Rating)
                            : 0);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(filterParams.Search))
            {
                var search = filterParams.Search.ToLower();
                predicate = predicate.And(cd =>
                    cd.Doctor.FirstName.ToLower().Contains(search) ||
                    cd.Doctor.LastName.ToLower().Contains(search) ||
                    cd.Doctor.User.Email!.ToLower().Contains(search) ||
                    cd.Doctor.User.PhoneNumber!.Contains(search));
            }

            AddCriteria(predicate);

            ApplyPagination(filterParams.PageSize * (filterParams.PageIndex - 1), filterParams.PageSize);
                return this;

        }

        

        public ClinicDoctorSpecifications GetDoctorsWithClinics(int doctorId)
        {
            AddCriteria(cd => cd.DoctorId == doctorId);
            AddInclude(cd => cd.Doctor);
            AddInclude(cd => cd.Clinic);
            return this;
        }

        public ClinicDoctorSpecifications GetClinicsWithDoctors(PaginationSpecParams paginationParams)
        {
            AddCriteria(cd=>cd.Clinic.IsApproved==true); 
            AddInclude(cd => cd.Clinic.User); 

            ApplyPagination(paginationParams.PageSize * (paginationParams.PageIndex - 1), paginationParams.PageSize);

            return this;
        }
    


    }
}
