using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Core.Entities;
using System.Linq.Expressions;

namespace SkinTelIigent.Core.Specification
{
    public class DoctorSpecifications : BaseSpecifications<Doctor>
    {
        public DoctorSpecifications() { }

        public DoctorSpecifications(int id)
            : base(d => d.Id == id)
        {
          
  
        }
        public DoctorSpecifications(int id, PaginationSpecParams paginationParams) : base(d => d.Id == id)
        {
            AddIncludeExpression(q => q.Include(d => d.DoctorPatients).ThenInclude(dp => dp.Patient));
            AddCriteria(d => string.IsNullOrWhiteSpace(paginationParams.Search) ||
                d.DoctorPatients.Any(dp =>
                    paginationParams.SearchWords.All(searchWord =>
                        dp.Patient.LastName.ToLower().Contains(searchWord) ||
                        (dp.Patient.Phone != null && dp.Patient.Phone.ToLower().Contains(searchWord))
                    )
                ));

            ApplyPagination(
                paginationParams.PageSize * (paginationParams.PageIndex - 1),
                paginationParams.PageSize
            );
        }

        public DoctorSpecifications(string id): base(d => d.UserId==id){}
        public DoctorSpecifications(PaginationSpecParams paginationParams)
         : base(d =>
             d.IsProfileCompleted &&
             (
                 string.IsNullOrWhiteSpace(paginationParams.Search) ||
                 paginationParams.SearchWords.All(searchWord =>
                     d.FirstName.ToLower().Contains(searchWord) ||
                     d.LastName.ToLower().Contains(searchWord) ||
                     (d.FirstName + " " + d.LastName).ToLower().Contains(searchWord) ||
                     d.AboutMe.ToLower().Contains(searchWord) ||
                     d.Address.ToLower().Contains(searchWord) ||
                     (d.User.PhoneNumber != null && d.User.PhoneNumber.ToLower().Contains(searchWord))
                 )
             )
         )
        {
            AddInclude(d => d.Reviews);
            IncludeExpressions.Add(d => d.Include(doctor => doctor.ClinicDoctors)
                                         .ThenInclude(cd => cd.Clinic));

            ApplyPagination(
                paginationParams.PageSize * (paginationParams.PageIndex - 1),
                paginationParams.PageSize
            );

            AddOrderBy(x => x.FirstName);
            AddOrderBy(x => x.CreatedDate);
        }
        public DoctorSpecifications(PaginationSpecParams paginationParams =null , bool includeOnlyRecent = false): base(d => includeOnlyRecent ? d.CreatedDate >= DateTime.UtcNow.AddDays(-7) : true)
        {
            AddInclude(d => d.Reviews);
            IncludeExpressions.Add(d => d.Include(doctor => doctor.ClinicDoctors)
                                         .ThenInclude(cd => cd.Clinic));
            if (paginationParams != null)
            {
                ApplyPagination(paginationParams.PageSize * (paginationParams.PageIndex - 1), paginationParams.PageSize);
                AddOrderByDescending(x => x.CreatedDate);
            }
        }

        public DoctorSpecifications CountDoctorCompletedBeforePagination()
        {
            AddCriteria(d => d.IsProfileCompleted == true);
            return this;
        }

    }
}
