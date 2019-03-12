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

        public override async Task HandleSaleDataAsync(SaleDataObtainmentResult dataObtainmentResult)
        {
            SaleManagementResult result;

            if (dataObtainmentResult.Success && dataObtainmentResult.SaleData.Sales.Count > 0)
            {                
                result = await salesDataManager.AddOrUpdateSaleDataAsync(dataObtainmentResult.SaleData);
                if (result.Succeeded)
                {
                    await salesDataManager.ErrorManager.RemoveErrorAsync(new SaleManagementResult() { FileName = dataObtainmentResult.SaleData.SourceFileName });
                    return;
                }                
            }
            else
            {
                result = new SaleManagementResult()
                {
                    FileName = dataObtainmentResult.SaleData.SourceFileName,
                    Succeeded = false,
                    ErrorMessage = dataObtainmentResult.ErrorMessage
                };
            }            

            await salesDataManager.ErrorManager.AddErrorAsync(result);            
        }

        
    }
}
