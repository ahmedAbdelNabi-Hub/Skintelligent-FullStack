using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Specification
{
    public class ClinicSpecifications :BaseSpecifications<Clinic>
    {
        public ClinicSpecifications()
        {
            
        }
        public ClinicSpecifications(int id) : base(c => c.Id == id){AddInclude(c => c.User);}
        public ClinicSpecifications(string userId) : base(c => c.UserId==userId) { }
        public ClinicSpecifications(PaginationSpecParams paginationParams)
        {
            AddCriteria(c => c.IsApproved == true);
            AddInclude(c => c.User);
            IncludeExpressions.Add(query => query.Include(cd => cd.ClinicDoctors).ThenInclude(c => c.Doctor.User));
            ApplyPagination(paginationParams.PageSize * (paginationParams.PageIndex - 1), paginationParams.PageSize);
        }

        public ClinicSpecifications GetNotApprovedClinics(PaginationSpecParams paginationParams)
        {
            AddCriteria(c => c.IsApproved == false && c.User.EmailConfirmed==true);
            AddInclude(c => c.User);
            ApplyPagination(paginationParams.PageSize * (paginationParams.PageIndex - 1), paginationParams.PageSize);

            return this;
        }

        public ClinicSpecifications CountApprovedClinic()
        {
            AddCriteria(c => c.IsApproved == true);
            return this;
        }
    }
}
