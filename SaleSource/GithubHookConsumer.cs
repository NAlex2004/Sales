using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Sales.SaleSource.Github;
using Sales.Storage.Management;
using Sales.SaleSource.Factory;

namespace Sales.SaleSource
{
    public class GithubHookConsumer : IHookConsumer
    {
        protected ISalesHandlerFactory salesHandlerFactory;
        private string token;
        private GithubHookParser hookParser;

        public GithubHookConsumer(ISalesHandlerFactory saleHandlerFactory, string githubRepoToken, GithubHookParser hookParser)
        {
            this.salesHandlerFactory = saleHandlerFactory;
            token = githubRepoToken;
            this.hookParser = hookParser;
        }        

        public async virtual Task ConsumeHookAsync(string hookJson)
        {
            if (salesHandlerFactory == null || hookParser == null)
            {
                return;
            }

            var fileEntries = hookParser.GetFileEntriesFromHook(hookJson);

            List<Task> tasks = new List<Task>();
            List<SalesHandlerBase> handlers = new List<SalesHandlerBase>();
            foreach (var entry in fileEntries)
            {
                SalesHandlerBase saleFileHandler = salesHandlerFactory.GetSalesHandler();
                handlers.Add(saleFileHandler);

                ISaleDataSource saleDataSource = new GithubSaleDataSource(entry, token);
                
                Task task = saleDataSource.GetSaleDataAsync()
                    .ContinueWith(async saleDataTask =>
                    {
                        var saleDataResult = saleDataTask.Result;
                        await saleFileHandler.HandleSaleDataAsync(saleDataResult);
                    });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);

            for (int handlerIndex = 0; handlerIndex < handlers.Count; handlerIndex++)
            {
                try
                {
                    handlers[handlerIndex]?.Dispose();
                }
                catch { }
            }
        }
    }
}
