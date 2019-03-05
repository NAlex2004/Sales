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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
        public async Task WebHook([FromBody] JObject hookJson)
        {            
            //JsonConvert.DeserializeObject()
            if (hookConsumer != null)
            {
                await hookConsumer.ConsumeHookAsync("");
            }
        }
    }
}
