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
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Threading;

namespace Sales.Storage.Management
{
    public partial class SaleDbDataManager : ISalesDataManager
    {
        protected ISalesUnitOfWork unitOfWork;
        static Mutex customersAndProductsSaveMutex = new Mutex(false, "CustomersProductsMutex");

        public SaleDbDataManager(ISalesUnitOfWork salesUnitOfWork)
        {
            unitOfWork = salesUnitOfWork ?? throw new ArgumentNullException();
        }

        public SaleDbDataManager() 
            : this(new SalesDbUnitOfWork(new SalesDbContext()))
        {
        }

        public async Task<SaleManagementResult> AddOrUpdateSaleDataAsync(SaleDataDto saleData)
        {
            if (saleData == null || string.IsNullOrEmpty(saleData.SourceFileName))
            {
                string message = saleData == null ? "saleData is null" : "SourceFileName is null or empty";
                return new SaleManagementResult() { Succeeded = false, ErrorMessage = message };
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

            SaleManagementResult addOrUpdateResult;
            addOrUpdateResult = sourceFile == null
                ? await AddSaleDataAsync(saleData)
                : await UpdateSaleDataAsync(saleData, sourceFile);

            return addOrUpdateResult;
        }

        protected IList<Sale> GetSalesWithSavedProductsAndCustomers(SourceFile sourceFile, IEnumerable<SaleDto> saleDetailsData)
        {
            List<Sale> sales = new List<Sale>();
            var mapper = Mappings.GetMapper();
            bool gotMutex = false;
            try
            {                
                foreach (var saleDto in saleDetailsData)
                {                    
                    Sale sale = mapper.Map<SaleDto, Sale>(saleDto);                    
                    sale.SourceFile = sourceFile;
                    gotMutex = customersAndProductsSaveMutex.WaitOne();
                    if (!gotMutex)
                    {
                        throw new ThreadInterruptedException("Could not get mutex.");
                    }
                    sale.Customer = unitOfWork.Customers.Add(sale.Customer);
                    sale.Product = unitOfWork.Products.Add(sale.Product);
                    unitOfWork.SaveChanges();

                    customersAndProductsSaveMutex.ReleaseMutex();
                    gotMutex = false;
                    sales.Add(sale);
                }

                return sales;
            }
            catch (Exception e)
            {
                unitOfWork.DiscardChanges();
                return new Sale[0];
            }
            finally
            {
                if (gotMutex)
                {
                    customersAndProductsSaveMutex.ReleaseMutex();
                }
            }
            
        }

        protected virtual async Task<SaleManagementResult> AddSaleDetailsDataAsync(SourceFile sourceFile, IList<SaleDto> saleDetailsData)
        {
            SaleManagementResult result = await Task.Run(() =>
            {
                var mapper = Mappings.GetMapper();
                try
                {
                    //foreach (var saleDto in saleDetailsData)
                    //{
                    //    Sale sale = mapper.Map<SaleDto, Sale>(saleDto);
                    //    sale.Customer = unitOfWork.Customers.Add(sale.Customer);
                    //    sale.Product = unitOfWork.Products.Add(sale.Product);
                    //    unitOfWork.Sales.Add(sale);
                    //}
                    var sales = GetSalesWithSavedProductsAndCustomers(sourceFile, saleDetailsData);
                    if (sales.Count == 0)
                    {
                        throw new ArgumentException("Empty sales list.");
                    }
                    unitOfWork.SourceFiles.Add(sourceFile);
                    unitOfWork.Sales.AddRange(sales);
                    unitOfWork.SaveChanges();

                    return new SaleManagementResult()
                    {
                        Succeeded = true                        
                    };
                }
                catch (Exception e)
                {
                    unitOfWork.DiscardChanges();
                    return new SaleManagementResult()
                    {
                        Succeeded = false,
                        ErrorMessage = GetLastErrorMessage(e)
                    };
                }
            });

            return result;
        }

        protected virtual async Task<SaleManagementResult> AddSaleDataAsync(SaleDataDto saleData)
        {
            SourceFile sourceFile = new SourceFile()
            {
                FileName = saleData.SourceFileName,
                FileDate = saleData.FileDate
            };            

            SaleManagementResult result = await AddSaleDetailsDataAsync(sourceFile, saleData.Sales);
            result.FileName = saleData.SourceFileName;

            return result;
        }

        protected virtual async Task<SaleManagementResult> UpdateSaleDataAsync(SaleDataDto saleData, SourceFile sourceFile)
        {
            var deleted = unitOfWork.Sales.Delete(sale => sale.SourceFileId == sourceFile.Id);
            unitOfWork.SourceFiles.Delete(file => file.Id == sourceFile.Id);
            //for (int i=0; i<saleData.Sales.Count; i++)
            //{
            //    saleData.Sales[i].SourceFileId = sourceFile.Id;
            //}
            sourceFile = new SourceFile()
            {
                FileDate = saleData.FileDate,
                FileName = saleData.SourceFileName
            };
            SaleManagementResult result = await AddSaleDetailsDataAsync(sourceFile, saleData.Sales);
            result.FileName = saleData.SourceFileName;

            return result;
        }

        public async Task<SaleManagementResult> AddErrorAsync(SaleManagementResult badResult)
        {
            SaleManagementResult result = new SaleManagementResult()
            {
                Succeeded = false,
                FileName = badResult.FileName                
            };

            try
            {
                unitOfWork.ErrorFiles.Add(new ErrorFile()
                {
                    FileName = badResult.FileName,
                    ErrorDescription = badResult.ErrorMessage
                });
                int savedCount = await unitOfWork.SaveChangesAsync();
                result.Succeeded = savedCount > 0;
                result.ErrorMessage = savedCount > 0 ? "" : "Data is not saved";                
            }
            catch (Exception e)
            {
                unitOfWork.DiscardChanges();
                result.ErrorMessage = GetLastErrorMessage(e);
            }

            return result;
        }

        private string GetLastErrorMessage(Exception exception)
        {
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            return exception.Message;
        }

        public async Task<SaleManagementResult> RemoveErrorAsync(SaleManagementResult badResult)
        {
            SaleManagementResult result = new SaleManagementResult()
            {
                FileName = badResult.FileName,
                Succeeded = false
            };

            try
            {
                var deleted = unitOfWork.ErrorFiles.Delete(error => error.FileName == badResult.FileName);
                if (deleted.Count() > 0)
                {
                    int savedCount = await unitOfWork.SaveChangesAsync();
                    result.Succeeded = savedCount > 0;
                    result.ErrorMessage = savedCount > 0 ? "" : "Data is not saved";                    

                    return result;
                }                
            }
            catch (Exception e)
            {
                unitOfWork.DiscardChanges();
                result.ErrorMessage = GetLastErrorMessage(e);                
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
