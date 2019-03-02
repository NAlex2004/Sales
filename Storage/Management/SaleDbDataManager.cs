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
    public partial class SaleDbDataManager : ISalesDataManager, IDisposable
    {
        protected ISalesUnitOfWork unitOfWork;

        public SaleDbDataManager(ISalesUnitOfWork salesUnitOfWork)
        {
            unitOfWork = salesUnitOfWork ?? throw new ArgumentNullException();
        }

        public SaleDbDataManager() 
            : this(new SalesDbUnitOfWork(new SalesDbContext()))
        {
        }

        public async Task<bool> AddOrUpdateSaleDataAsync(SaleDataDto saleData)
        {
            if (saleData == null || string.IsNullOrEmpty(saleData.SourceFileName))
            {
                return false;
            }

            // Group data
            saleData.Sales = saleData.Sales
                .GroupBy(sale => new { sale.CustomerName, sale.ProductName, sale.SaleDate },
                (key, sales) => new SaleDto()
                {
                    CustomerName = key.CustomerName,
                    ProductName = key.ProductName,
                    SaleDate = key.SaleDate,
                    TotalSum = sales.Sum(s => s.TotalSum)
                }).ToList();

            SourceFile sourceFile = await unitOfWork.SourceFiles.Get(file => file.FileName.Equals(saleData.SourceFileName)).FirstOrDefaultAsync();

            bool addOrUpdateResult = false;
            addOrUpdateResult = sourceFile == null
                ? await AddSaleDataAsync(saleData)
                : await UpdateSaleDataAsync(saleData, sourceFile);

            return addOrUpdateResult;
        }

        protected virtual async Task<bool> AddSaleDetailsDataAsync(IList<SaleDto> saleDetailsData)
        {
            bool result = await Task.Run(() =>
            {
                var mapper = Mappings.GetMapper();
                try
                {
                    foreach (var saleDto in saleDetailsData)
                    {
                        Sale sale = mapper.Map<SaleDto, Sale>(saleDto);
                        sale.Customer = unitOfWork.Customers.Add(sale.Customer);
                        sale.Product = unitOfWork.Products.Add(sale.Product);
                        unitOfWork.Sales.Add(sale);
                    }

                    unitOfWork.SaveChanges();

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            });

            return result;
        }

        protected virtual async Task<bool> AddSaleDataAsync(SaleDataDto saleData)
        {
            SourceFile sourceFile = new SourceFile()
            {
                FileName = saleData.SourceFileName
            };
            unitOfWork.SourceFiles.Add(sourceFile);

            bool result = await AddSaleDetailsDataAsync(saleData.Sales);

            return result;
        }

        protected virtual async Task<bool> UpdateSaleDataAsync(SaleDataDto saleData, SourceFile sourceFile)
        {
            var deleted = unitOfWork.Sales.Delete(sale => sale.SourceFileId == sourceFile.Id);  
            for (int i=0; i<saleData.Sales.Count; i++)
            {
                saleData.Sales[i].SourceFileId = sourceFile.Id;
            }
            bool result = await AddSaleDetailsDataAsync(saleData.Sales);            
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
