using Sales.SalesEntity.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.DAL.Database
{
    public class ProductsDbRepository : GenericDbRepository<Product>
    {
        public ProductsDbRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public override Product Add(Product entity)
        {
            Product localProduct = GetLocalEntity<Product>(p => p.ProductName.Equals(entity.ProductName));
                
            if (localProduct != null)
            {                                
                return localProduct;
            }

            Product existing = Get(product => product.ProductName.Equals(entity.ProductName)).FirstOrDefault();
            if (existing != null)
            {
                return existing;
            }

            return base.Add(entity);
        }
    }
}
