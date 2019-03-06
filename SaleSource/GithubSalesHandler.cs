using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.Storage.Management;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections;
using Sales.Storage.DTO;
using Sales.SaleSource.Github;
using System.IO;

namespace Sales.SaleSource
{
    public class GithubSalesHandler : SalesHandlerBase
    {        
        public GithubSalesHandler(ISalesDataManager salesDataManager) : base(salesDataManager)
        {            
        }

        public override async Task HandleSaleSourceAsync(ISaleDataSource saleDataSource)
        {
            SaleDataDto saleData = await saleDataSource.GetSaleDataAsync();
            SaleManagementResult result;

            if (saleData.Sales != null && saleData.Sales.Count > 0)
            {                
                result = await salesDataManager.AddOrUpdateSaleDataAsync(saleData);
                if (result.Succeeded)
                {
                    await salesDataManager.ErrorManager.RemoveErrorAsync(new SaleManagementResult() { FileName = saleData.SourceFileName });
                    return;
                }                
            }
            else
            {
                result = new SaleManagementResult()
                {
                    FileName = saleData.SourceFileName,
                    Succeeded = false,
                    ErrorMessage = "[GithubSaleFileHandler.HandleSaleFileAsync]: Skipping file because it has no suitable data."
                };
            }            

            await salesDataManager.ErrorManager.AddErrorAsync(result);            
        }

        
    }
}
