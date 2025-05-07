using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Specification
{
    public class EntityWithUserSpecification<T> : BaseSpecifications<T> where T : BaseEntity , IUserAssociated 
    {
        public EntityWithUserSpecification(int id):base(x=>x.Id == id)
        {
            AddInclude(x=>x.User);  
        }

    }
}
