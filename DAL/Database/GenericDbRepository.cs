using Sales.DAL.Interfaces;
using Sales.SalesEntity.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sales.DAL.Database
{
    public abstract class GenericDbRepository<TEntity> : IRepository<TEntity> where TEntity: class
    {
        protected DbContext dbContext;

        public GenericDbRepository(DbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException();
        }

        public virtual TEntity Add(TEntity entity)
        {            
            return dbContext.Set<TEntity>().Add(entity);
        }

        public virtual IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            // Not using dbContext.Set<TEntity>().AddRange because of overriding Add method of repository. Adding product with same name just return existing, for example
            List<TEntity> added = new List<TEntity>();
            foreach(var entity in entities)
            {
                added.Add(Add(entity));
            }

            return added;
        }

        public virtual IEnumerable<TEntity> Delete(Expression<Func<TEntity, bool>> condition)
        {
            // Local entities, not saved to database
            //var localEntities = dbContext.Set<TEntity>().Local.Where(condition.Compile()).ToArray();
            //foreach (var entity in localEntities)
            //{
            //    EntityState currentState = dbContext.Entry(entity).State;
            //    if (currentState == EntityState.Added || currentState == EntityState.Unchanged || currentState == EntityState.Modified)
            //    {
            //        dbContext.Entry(entity).State = EntityState.Deleted;
            //    }
            //}

            var entitiesToRemove = Get(condition).ToArray();            
            return dbContext.Set<TEntity>().RemoveRange(entitiesToRemove);
        }

        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> condition = null)
        {
            return condition == null
                ? dbContext.Set<TEntity>()
                : dbContext.Set<TEntity>().Where(condition);
        }

        protected TEntity GetLocalEntity<TEntity>(Func<TEntity, bool> predicate) where TEntity: class
        {            
            TEntity localEntity = dbContext.Set<TEntity>().Local.FirstOrDefault(predicate);
            if (localEntity != null)
            {
                if (dbContext.Entry(localEntity).State == EntityState.Deleted)
                {
                    dbContext.Entry(localEntity).State = EntityState.Unchanged;
                }                
            }

            return localEntity;
        }
    }
}
