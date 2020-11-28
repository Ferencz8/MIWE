using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MIWE.Data.Services
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filterExpression = null);

        Task<TEntity> GetById(object id);

        Task<TEntity> Create(TEntity entity);

        Task CreateInBulk(IEnumerable<TEntity> entities);

        Task Update(TEntity entity);

        Task Delete(object id);
    }
}

