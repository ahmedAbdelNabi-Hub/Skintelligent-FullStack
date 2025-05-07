using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Infrastructure.Data;
using SkinTelIigent.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {

         private IDbContextTransaction _transaction;
         private readonly SkinTelIigentDbContext _dbContext; 
         private Dictionary<string, object> repositories;
         private IAppointmentRepository _Appointmentrepository;
        private IAnalysisRepository _analysisRepository;


        public UnitOfWork(SkinTelIigentDbContext dbContext)
        {
            _dbContext = dbContext;  
            repositories = new Dictionary<string, object>();    
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync(); 
        }

        public async Task<bool> CommitAsync()
        {
            try
            {
                await _transaction.CommitAsync();   
                return true;    
            }
            catch { 
              await _transaction.RollbackAsync();
              return false;  
            }
            finally 
            {
               await _transaction.DisposeAsync();
                _transaction = null!; 
            }
                
        }
        public async Task RollbackAsync()
        {
            if (_transaction != null) { 
              await _dbContext.Database.RollbackTransactionAsync();   
              await _transaction.DisposeAsync();
                _transaction = null!;
            }
        }
        public async Task<int> SaveChangeAsync()
        {
             return await _dbContext.SaveChangesAsync();  
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var nameEntity = typeof(TEntity).Name;  
            if(!repositories.ContainsKey(nameEntity))
            {
                if (typeof(TEntity) == typeof(Appointment))
                {
                    repositories.Add(nameEntity, GetAppointmentRepository());
                }
                else
                {
                    var typeEntity = typeof(GenericRepository<>);
                    var Instance = Activator.CreateInstance(typeEntity.MakeGenericType(typeof(TEntity)), _dbContext);
                    repositories.Add(nameEntity, Instance!);
                }
            } 
            return (IGenericRepository<TEntity>) repositories[nameEntity]; 
        }

        private IAppointmentRepository GetAppointmentRepository()
        {
            _Appointmentrepository ??= new AppointmentRepository(_dbContext);
            return _Appointmentrepository;
        }

        public IAnalysisRepository AnalysisRepository()
        {
            _analysisRepository ??= new AnalysisRepository(_dbContext);
            return _analysisRepository;
        }



    }
}
