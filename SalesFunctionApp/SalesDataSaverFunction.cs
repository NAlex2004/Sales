using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Sales.SaleSource;
using Sales.SaleSource.Factory;

namespace Sales.SalesFunctionApp
{
    public static class SalesDataSaverFunction
    {
        [FunctionName("SalesDataSaverFunction")]
        public static async Task Run([QueueTrigger("SalesDataQueue")]SaleDataObtainmentResult saleDataObtainmentResult, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: SalesDataSaverFunction");

            ISalesHandlerFactory handlerFactory = new GithubSalesHandlerFactory();
            SalesHandlerBase salesHandler = handlerFactory.GetSalesHandler();
            await salesHandler.HandleSaleDataAsync(saleDataObtainmentResult);
        }
    }
}
