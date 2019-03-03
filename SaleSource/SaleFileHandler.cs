using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.Storage.Management;
using System.Net;

namespace Sales.SaleSource
{
    public class SaleFileHandler : SaleFileHandlerBase, IDisposable
    {
        public SaleFileHandler(ISalesDataManager salesDataManager) : base(salesDataManager)
        {
        }

        public override Task HandleSaleFileAsync(string location)
        {
            throw new NotImplementedException();
        }

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
