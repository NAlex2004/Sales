using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sales.DAL.Interfaces
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> condition = null);
        TEntity Add(TEntity entity);
        IEnumerable<TEntity> Delete(Expression<Func<TEntity, bool>> condition);
        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities);
    }
}
