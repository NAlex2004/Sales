using Sales.SaleSource;
using Sales.Storage.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Classes
{
    public class EmptySaleFileHandlerTestClass : SalesHandlerBase
    {
        public EmptySaleFileHandlerTestClass() : base(new SaleDbDataManager())
        {
        }

        public override Task HandleSaleDataAsync(SaleDataObtainmentResult dataObtainmentResult)
        {
            return Task.Delay(100);
        }
    }
}
