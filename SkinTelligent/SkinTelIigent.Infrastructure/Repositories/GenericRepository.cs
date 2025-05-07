using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Core.Specification;
using SkinTelIigent.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SkinTelIigent.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly SkinTelIigentDbContext _dbContext;

        public GenericRepository(SkinTelIigentDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity)
        {
            var trackedEntity = _dbContext.Set<T>().Local.FirstOrDefault(e => e.Equals(entity));
            if (trackedEntity == null)
            {
                _dbContext.Attach(entity);
            }

            _dbContext.Entry(entity).State = EntityState.Modified;

            foreach (var navigation in _dbContext.Entry(entity).Navigations)
            {
                if (navigation.IsModified)
                {
                    _dbContext.Entry(navigation.CurrentValue!).State = EntityState.Modified;
                }
            }

            return Task.CompletedTask;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> GetByIdSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec, bool asTracking = false)
        {
            var query = ApplySpecification(spec);
            if (!asTracking)
                query = query.AsNoTracking(); 
            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }
        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            _dbContext.Set<T>().RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task<int> CountWithSpec(ISpecification<T> spec)
        {
            ArgumentNullException.ThrowIfNull(spec);
            return await ApplySpecification(spec).CountAsync();
        }
        public IQueryable<T> GetQueryableWithSpec(ISpecification<T> spec)
        {
            return ApplySpecification(spec); 
        }
        public async Task<List<TResult>> GetProjectedAsync<TResult>(Expression<Func<T, TResult>> selector, ISpecification<T> spec)
        {
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentNullException.ThrowIfNull(spec);

            return await ApplySpecification(spec)
                         .AsNoTracking()
                         .Select(selector)
                         .ToListAsync();
        }
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            ArgumentNullException.ThrowIfNull(spec);
            return SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }
    }
}
