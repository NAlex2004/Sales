using Sales.Storage.Management;
using System;
using System.Threading.Tasks;

namespace Sales.SaleSource
{
    public abstract class SaleFileHandlerBase: IDisposable
    {
        protected ISalesDataManager salesDataManager;
        public SaleFileHandlerBase(ISalesDataManager salesDataManager)
        {
            this.salesDataManager = salesDataManager ?? throw new ArgumentNullException();
        }

        public abstract Task HandleSaleFileAsync(string location);

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