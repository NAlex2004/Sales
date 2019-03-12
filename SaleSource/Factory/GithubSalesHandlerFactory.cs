using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.DAL.Database;
using Sales.SalesEntity.Entity;
using Sales.SaleSource;
using Sales.Storage.Management;

namespace Sales.SaleSource.Factory
{
    [Obsolete]
    public class GithubSalesHandlerFactory : ISalesHandlerFactory
    {
        public SalesHandlerBase GetSalesHandler()
        {
            return new GithubSalesHandler(new SaleDbDataManager(new SalesDbUnitOfWork(new SalesDbContext())));
        }
    }
}
