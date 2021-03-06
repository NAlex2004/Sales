﻿using System;
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
        public  Task WebHook([FromBody] JObject hookJObject)
        {                        
            if (hookConsumer != null && hookJObject != null)
            {
                return hookConsumer.ConsumeHookAsync(hookJObject.ToString());
            }

            return Task.FromResult(0);
        }
    }
}
