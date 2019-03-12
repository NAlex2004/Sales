using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Sales.SaleSource;
using Sales.SaleSource.Factory;
using Sales.SaleSource.Validation;

namespace Sales.SalesFunctionApp
{
    public static class SalesWebHookFunction
    {
        [FunctionName("SalesWebHookFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log,
            [Queue("SalesHookQueue")] ICollector<string> hookQueue)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // Get request body
            string hookJson = await req.Content.ReadAsStringAsync().ConfigureAwait(false);

            hookQueue.Add(hookJson);       
            return req.CreateResponse(HttpStatusCode.OK, "Hook accepted");
        }
    }
}
