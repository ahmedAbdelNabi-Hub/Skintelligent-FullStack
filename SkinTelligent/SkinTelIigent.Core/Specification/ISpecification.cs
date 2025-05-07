using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SkinTelIigent.Core.Entities;

namespace SkinTelIigent.Core.Specification
{
    public interface ISpecification<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; }
        public List<Func<IQueryable<T>, IQueryable<T>>> IncludeExpressions { get; }
        public List<Expression<Func<T, object>>> Includes { get; }
        public Expression<Func<T, object>> GroupByExpression { get; }
        public Expression<Func<T, object>> OrderByDescending { get; }
        public Expression<Func<T, object>> OrderBy { get; }
        public int Skip { get; }
        public int Take { get; }
        public bool IsPaginationEnabled { get; }
    }
}
