using Sales.Storage.Management;
using System.Threading.Tasks;

namespace Sales.SaleSource
{
    public abstract class SaleFileHandlerBase
    {
        protected ISalesDataManager salesDataManager;
        public SaleFileHandlerBase(ISalesDataManager salesDataManager)
        {
            this.salesDataManager = salesDataManager;
        }

        public abstract Task HandleSaleFileAsync(string location);
    }
}