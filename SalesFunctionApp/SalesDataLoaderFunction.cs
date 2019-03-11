using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Sales.SaleSource;
using Sales.SaleSource.Github;

namespace Sales.SalesFunctionApp
{
    public static class SalesDataLoaderFunction
    {
        [FunctionName("SalesDataLoaderFunction")]
        public static void Run([QueueTrigger("SalesFileEntriesQueue")]GithubFileEntry fileEntry, TraceWriter log,
            [Queue("SalesDataQueue")] ICollector<SaleDataObtainmentResult> saleDataQueue)
        {
            log.Info($"C# Queue trigger function processed: SalesDataLoaderFunction");
        }
    }
}
