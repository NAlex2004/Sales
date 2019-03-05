using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Sales.SaleSource.Factory;
using Sales.SaleSource.Github;
using Sales.SaleSource;

namespace Sales.SalesWebApp.Controllers
{
    public class SalesController : ApiController
    {
        IHookConsumer hookConsumer;

        public SalesController(IHookConsumer hookConsumer)
        {
            this.hookConsumer = hookConsumer;
        }

        [HttpPost]
        public async Task WebHook([FromBody] string hookJson)
        {
            if (hookConsumer != null)
            {
                await hookConsumer.ConsumeHookAsync(hookJson);
            }
        }
    }
}
