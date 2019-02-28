using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Sales.SalesEntity.Entity;
using Sales.Storage.DTO;
using Sales.DAL.Interfaces;
using Sales.DAL.Database;

namespace Sales.Storage.Management
{
    public class SaleDbDataManager : ISalesDataManager, IDisposable
    {
        protected ISalesUnitOfWork unitOfWork;

        public SaleDbDataManager(ISalesUnitOfWork salesUnitOfWork)
        {
            unitOfWork = unitOfWork ?? throw new ArgumentNullException();            
        }

        public SaleDbDataManager() : this(new SalesDbUnitOfWork(new SalesDbContext()))
        {
        }

        public async Task<bool> AddOrUpdateSaleDataAsync(SaleDataDto saleData)
        {
            SourceFile sourceFile = await unitOfWork.SourceFiles.Get(file => file.FileName.Equals(saleData.SourceFileName)).FirstOrDefaultAsync();

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
                    if (unitOfWork != null)
                    {
                        unitOfWork.Dispose();
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
