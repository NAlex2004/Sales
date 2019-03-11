using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Sales.SaleSource;
using Sales.SaleSource.Github;

namespace Sales.SalesFunctionApp
{
    public static class SalesDataLoaderFunction
    {
        [FunctionName("SalesDataLoaderFunction")]
        public static async Task Run([QueueTrigger("SalesFileEntriesQueue")]GithubFileEntry fileEntry, TraceWriter log,
            [Queue("SalesDataQueue")] ICollector<SaleDataObtainmentResult> saleDataQueue)
        {
            log.Info($"C# Queue trigger function processed: SalesDataLoaderFunction");

            string token = ConfigurationManager.AppSettings["token"];
            ISaleDataSource saleDataSource = new GithubSaleDataSource(fileEntry, token);
            SaleDataObtainmentResult obtainmentResult = await saleDataSource.GetSaleDataAsync();
            if (obtainmentResult.Success)
            {
                log.Info($"[SalesDataLoaderFunction]: loaded {fileEntry.Url}");                
            }
            else
            {
                log.Error($"[SalesDataLoaderFunction]: ERROR loading {fileEntry.Url} Reason: {obtainmentResult.ErrorMessage}");
            }

            saleDataQueue.Add(obtainmentResult);
        }
    }
}
