﻿using Sales.DAL.Interfaces;
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
            return dbContext.Set<TEntity>().AddRange(entities);
        }

        public virtual IEnumerable<TEntity> Delete(Expression<Func<TEntity, bool>> condition)
        {
            var entitiesToRemove = Get(condition).ToArray();            
            return dbContext.Set<TEntity>().RemoveRange(entitiesToRemove);
        }

        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> condition = null)
        {
            return condition == null
                ? dbContext.Set<TEntity>()
                : dbContext.Set<TEntity>().Where(condition);
        }
    }
}
