using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Sales.SalesFunctionApp.Dependency;
using Sales.SaleSource;
using Sales.SaleSource.Factory;
using Unity;

namespace Sales.SalesFunctionApp
{
    public static class SalesDataSaverFunction
    {
        [FunctionName("SalesDataSaverFunction")]
        public static async Task Run([QueueTrigger("SalesDataQueue")]SaleDataObtainmentResult saleDataObtainmentResult, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: SalesDataSaverFunction");

            ISalesHandlerFactory handlerFactory = DependencyContainer.Container.Resolve<ISalesHandlerFactory>();
            SalesHandlerBase salesHandler = handlerFactory.GetSalesHandler();
            await salesHandler.HandleSaleDataAsync(saleDataObtainmentResult);
        }
    }
}
