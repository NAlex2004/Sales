using Sales.SalesEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Tests
{
    public class TestDbContext : SalesDbContext
    {
        public TestDbContext() : base()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<TestDbContext>());
        }
    }
}
