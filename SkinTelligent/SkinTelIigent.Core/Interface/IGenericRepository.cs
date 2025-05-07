using Microsoft.EntityFrameworkCore.ChangeTracking;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Interface
{
    public interface IGenericRepository<T> where T : BaseEntity
    {

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> Spec, bool asTracking = false);
        Task<T> GetByIdSpecAsync(ISpecification<T> Spec);
        Task<List<TResult>> GetProjectedAsync<TResult>(Expression<Func<T, TResult>> selector, ISpecification<T> spec);
        Task<IReadOnlyList<T>> GetAllAsync();
        IQueryable<T> GetQueryableWithSpec(ISpecification<T> spec);
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);
        Task<int> CountWithSpec(ISpecification<T> Spec);

    }
}
