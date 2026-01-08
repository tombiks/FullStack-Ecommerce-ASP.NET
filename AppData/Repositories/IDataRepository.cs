using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Repositories
{

    public interface IDataRepository
    {
        Task<IEnumerable<T>> GetAll<T>() where T : class;
        Task<T?> GetById<T>(int id) where T : class;
        Task<bool> Add<T>(T entity) where T : class;
        Task<bool> Update<T>(T entity) where T : class;
        Task<bool> Delete<T>(T entity) where T : class;
        Task<bool> DeleteById<T>(int id) where T : class;
    }

    internal class DataRepository : IDataRepository
    {
        private readonly DbContext _dbContext;

        public DataRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Add<T>(T entity) where T : class
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete<T>(T entity) where T : class
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteById<T>(int id) where T : class
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);

            if (entity == null)
            {
                return false;
            }

            _dbContext.Set<T>().Remove(entity);

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<T>> GetAll<T>() where T : class
        {
            var entities = await _dbContext.Set<T>().ToListAsync();

            return entities;
        }

        public async Task<T?> GetById<T>(int id) where T : class
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<bool> Update<T>(T entity) where T : class
        {
            _dbContext.Set<T>().Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
