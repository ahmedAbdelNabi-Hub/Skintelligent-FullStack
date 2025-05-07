using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SkinTelIigent.Core.Entities;

namespace SkinTelIigent.Core.Specification.PatientSpecific
{
    public class PatientSpecification : BaseSpecifications<Patient>
    {
        public PatientSpecification()
        {
           
        }

        public PatientSpecification(int id) : base(p => p.Id == id){AddInclude(P => P.User);}
        public PatientSpecification(string userId) : base(p => p.UserId==userId){}
        public PatientSpecification(PaginationSpecParams specParams)
        {
            ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
      
        }


        public PatientSpecification(Expression<Func<Patient, bool>> criteria) : base(criteria)
        {
            AddInclude(p => p.User);
        }


        public PatientSpecification GetAllReportsByPatientId(int id)
        {
            AddCriteria(p => p.Id == id);
            AddInclude(p => p.Reports);
            return this;
        }
 
    }
}


