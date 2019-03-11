using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Sales.SaleSource.Github;

namespace Sales.SalesFunctionApp
{
    public static class SalesHookParsingFunction
    {
        [FunctionName("SalesHookParsingFunction")]
        public static void Run([QueueTrigger("SalesHookQueue")]string hookJson, TraceWriter log,
            [Queue("SalesFileEntriesQueue")] ICollector<GithubFileEntry> fileEntriesQueue)
        {
            log.Info($"C# Queue trigger function processed: hook");
            GithubFileEntry fileEntry = new GithubFileEntry()
            {
                CommitDate = DateTime.Now,
                State = FileState.Added,
                Url = "no.com"
            };

            fileEntriesQueue.Add(fileEntry);
        }
    }
}
