using MIWE.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MIWE.Data.Services
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {

        protected readonly WorkerContext _dbContext;

        public GenericRepository(WorkerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filterExpression = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            if (filterExpression != null)
            {
                query = query.Where(filterExpression);
            }
            return query;
        }

        public virtual async Task<TEntity> GetById(object id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task CreateInBulk(IEnumerable<TEntity> entities)
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TEntity> Create(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(object id)
        {
            var entity = await GetById(id);
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
