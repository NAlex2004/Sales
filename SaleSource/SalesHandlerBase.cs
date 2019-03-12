using Sales.Storage.Management;
using System;
using System.Threading.Tasks;

namespace Sales.SaleSource
{
    public abstract class SalesHandlerBase: IDisposable
    {
        protected ISalesDataManager salesDataManager;
        public SalesHandlerBase(ISalesDataManager salesDataManager)
        {
            this.salesDataManager = salesDataManager ?? throw new ArgumentNullException();
        }

        public abstract Task HandleSaleDataAsync(SaleDataObtainmentResult dataObtainmentResult);
        //public abstract Task HandleSaleSourceAsync(ISaleDataSource saleDataSource);

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    salesDataManager.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}