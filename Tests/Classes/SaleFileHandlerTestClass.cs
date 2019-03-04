using Sales.SaleSource;
using Sales.Storage.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Classes
{
    public class SaleFileHandlerTestClass : SaleFileHandlerBase
    {
        public SaleFileHandlerTestClass() : base(new SaleDbDataManager())
        {
        }

        public override Task HandleSaleFileAsync(string location)
        {
            return Task.Delay(100);
        }
    }
}
