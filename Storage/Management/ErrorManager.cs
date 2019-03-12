using Sales.DAL.Interfaces;
using Sales.SalesEntity.Entity;
using Sales.Storage.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Storage.Management
{
    public class ErrorManager
    {
        private ISalesUnitOfWork unitOfWork;

        public ErrorManager(ISalesUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException();
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
                int savedCount = await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                result.Succeeded = savedCount > 0;
                result.ErrorMessage = savedCount > 0 ? "" : "Data is not saved";
            }
            catch (Exception e)
            {
                unitOfWork.DiscardChanges();
                result.ErrorMessage = e.GetLastInnerExceptionMessage();
            }

            return result;
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
                    int savedCount = await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    result.Succeeded = savedCount > 0;
                    result.ErrorMessage = savedCount > 0 ? "" : "Data is not saved";

                    return result;
                }
            }
            catch (Exception e)
            {
                unitOfWork.DiscardChanges();
                result.ErrorMessage = e.GetLastInnerExceptionMessage();
            }

            return result;
        }
    }
}
