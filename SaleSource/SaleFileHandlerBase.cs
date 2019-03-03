using Sales.Storage.Management;
using System;
using System.Threading.Tasks;

namespace Sales.SaleSource
{
    public abstract class SaleFileHandlerBase
    {
        protected ISalesDataManager salesDataManager;
        public SaleFileHandlerBase(ISalesDataManager salesDataManager)
        {
            this.salesDataManager = salesDataManager ?? throw new ArgumentNullException();
        }

        public abstract Task HandleSaleFileAsync(string location);
    }
}