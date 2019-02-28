using Sales.DAL.Interfaces;
using Sales.SalesEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.DAL.Database
{
    public class SalesDbUnitOfWork : ISalesUnitOfWork
    {
        public IRepository<Customer> Customers { get; protected set; }
        public IRepository<ErrorFile> ErrorFiles { get; protected set; }
        public IRepository<Product> Products { get; protected set; }
        public IRepository<Sale> Sales { get; protected set; }
        public IRepository<SourceFile> SourceFiles { get; protected set; }

        protected SalesDbContext dbContext;

        public SalesDbUnitOfWork(SalesDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException();
            Customers = new CustomersDbRepository(this.dbContext);
            ErrorFiles = new ErrorFilesDbRepository(this.dbContext);
            Products = new ProductsDbRepository(this.dbContext);
            Sales = new SalesDbRepository(this.dbContext);
            SourceFiles = new SourceFilesDbRepository(this.dbContext);
        }

        public int SaveChanges()
        {
            return dbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return dbContext.SaveChangesAsync();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }

                disposedValue = true;
            }
        }
      
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        #endregion
    }
}
