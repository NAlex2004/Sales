using Sales.SalesEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.DAL.Interfaces
{
    public interface ISalesUnitOfWork : IDisposable
    {
        IRepository<Customer> Customers { get; }
        IRepository<ErrorFile> ErrorFiles { get; }
        IRepository<Product> Products { get; }
        IRepository<Sale> Sales { get; }
        IRepository<SourceFile> SourceFiles { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
        void DiscardChanges();
    }
}
