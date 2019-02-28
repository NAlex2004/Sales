using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Sales.SalesEntity.Entity;
using Sales.Storage.DTO;

namespace Sales.Storage.Management
{
    public class SaleDbDataManager : ISalesDataManager, IDisposable
    {
        protected SalesDbContext dbContext;

        public SaleDbDataManager(SalesDbContext dbContext)
        {
            dbContext = dbContext ?? throw new ArgumentNullException();            
        }

        public SaleDbDataManager() : this(new SalesDbContext())
        {            
        }

        public async Task<bool> AddOrUpdateSaleDataAsync(SaleDataDto saleData)
        {
            SourceFile sourceFile = await dbContext.SourceFiles.FirstOrDefaultAsync(file => file.FileName.Equals(saleData.SourceFileName));

            bool addOrUpdateResult = false;
            if (sourceFile != null)
            {
                addOrUpdateResult = await UpdateSaleData(saleData, sourceFile);
            }
            else
            {

            }

            return addOrUpdateResult;
        }

        protected virtual async Task<bool> UpdateSaleData(SaleDataDto saleData, SourceFile sourceFile)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }
               
                disposedValue = true;
            }
        }        

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);         
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
