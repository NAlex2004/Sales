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
using Sales.Storage.Validation;

namespace Sales.Storage.Management
{
    public partial class SaleDbDataManager : ISalesDataManager, IDisposable
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
            var validationResult = FileNameValidator.Validate(saleData.SourceFileName);
            if (!validationResult.IsValid)
            {
                return false;
            }

            SourceFile sourceFile = await unitOfWork.SourceFiles.Get(file => file.FileName.Equals(saleData.SourceFileName)).FirstOrDefaultAsync();

            bool addOrUpdateResult = false;
            addOrUpdateResult = sourceFile == null
                ? await AddSaleDataAsync(saleData)
                : await UpdateSaleDataAsync(saleData, sourceFile);

            return addOrUpdateResult;
        }

        protected virtual async Task<bool> AddSaleDetailsDataAsync(IList<SaleDto> saleDetailsData)
        {
            throw new NotImplementedException();
        }

        protected virtual async Task<bool> AddSaleDataAsync(SaleDataDto saleData)
        {
            
            throw new NotImplementedException();
        }

        protected virtual async Task<bool> UpdateSaleDataAsync(SaleDataDto saleData, SourceFile sourceFile)
        {
            bool result = await Task.Run(() =>
            {
                var deleted = unitOfWork.Sales.Delete(sale => sale.SourceFileId == sourceFile.Id);
                return deleted.Count() > 0;
            });
            
            if (result)
            {
                result = await AddSaleDetailsDataAsync(saleData.Sales);
            }

            return result;
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
