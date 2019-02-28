using Sales.SalesEntity.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.DAL.Database
{
    public class CustomersDbRepository : GenericDbRepository<Customer>
    {
        public CustomersDbRepository(DbContext dbContext) : base(dbContext)
        {            
        }

        public override Customer Add(Customer entity)
        {
            Customer localCustomer = GetLocalEntity<Customer>(p => p.CustomerName.Equals(entity.CustomerName));

            if (localCustomer != null)
            {
                return localCustomer;
            }

            Customer existing = Get(customer => customer.CustomerName.Equals(entity.CustomerName)).FirstOrDefault();
            if (existing != null)
            {
                return existing;
            }

            return base.Add(entity);
        }
    }
}
