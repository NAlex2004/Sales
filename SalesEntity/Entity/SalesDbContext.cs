using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace NAlex.SalesEntity.Entity
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext() : base("Sales")
        {
        }

        public DbSet<SourceFile> SourceFiles { get; set; }
        public DbSet<ErrorFile> ErrorFiles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sale> Sales { get; set; }
    }
}
