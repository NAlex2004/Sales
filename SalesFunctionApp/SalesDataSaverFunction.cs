using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Sales.SaleSource;

namespace Sales.SalesFunctionApp
{
    public static class SalesDataSaverFunction
    {
        [FunctionName("SalesDataSaverFunction")]
        public static void Run([QueueTrigger("SalesDataQueue")]SaleDataObtainmentResult saleDataObtainmentResult, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: SalesDataSaverFunction");
        }
    }
}
