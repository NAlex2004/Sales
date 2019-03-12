using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Sales.SaleSource;
using Sales.SaleSource.Github;
using Sales.SaleSource.Validation;
using Sales.SalesFunctionApp.Dependency;
using Unity;

namespace Sales.SalesFunctionApp
{
    public static class SalesHookParsingFunction
    {
        [FunctionName("SalesHookParsingFunction")]
        public static void Run([QueueTrigger("SalesHookQueue")]string hookJson, TraceWriter log,
            [Queue("SalesFileEntriesQueue")] ICollector<GithubFileEntry> fileEntriesQueue)
        {
            log.Info($"C# Queue trigger function processed: SalesHookParsingFunction");

            GithubHookParser hookParser = DependencyContainer.Container.Resolve<GithubHookParser>();
            var fileEntries = hookParser.GetFileEntriesFromHook(hookJson);

            foreach (var fileEntry in fileEntries)
            {
                log.Info($"[SalesHookParsingFunction]: added entry to queue {fileEntry.Url}");
                fileEntriesQueue.Add(fileEntry);
            }            
        }
    }
}
