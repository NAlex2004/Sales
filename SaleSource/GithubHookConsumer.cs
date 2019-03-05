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
        private const string GITHUB_URL = "https://api.github.com/repos/";
        protected ISalesHandlerFactory salesHandlerFactory;
        protected Func<string, bool> fileNameValidator;
        private string token;

        public GithubHookConsumer(ISalesHandlerFactory saleHandlerFactory, string githubRepoToken, Func<string, bool> fileNameValidator = null)
        {
            this.salesHandlerFactory = saleHandlerFactory;
            this.fileNameValidator = fileNameValidator;
            token = githubRepoToken;
        }

        protected string MakeGetUrl(string filePath, GithubRepository repository)
        {
            StringBuilder urlBuilder = new StringBuilder(GITHUB_URL);
            urlBuilder.Append(repository.Full_Name).Append("/contents/").Append(filePath);
            return urlBuilder.ToString();
        }        

        private void ProcessCommitFilesWithState(Dictionary<string, GithubFileEntry> urls, IList<string> fileNames, FileState fileState, DateTime commitDate, GithubRepository repository)
        {
            foreach (string fileName in fileNames)
            {
                if (string.IsNullOrEmpty(fileName)
                    || (fileNameValidator != null && !fileNameValidator(Path.GetFileName(fileName))))
                {                                        
                    continue;                    
                }

                string url = MakeGetUrl(fileName, repository);
                if (urls.ContainsKey(url))
                {
                    var entry = urls[url];
                    entry.State = fileState;
                    entry.CommitDate = commitDate;
                }
                else
                {
                    urls.Add(url, new GithubFileEntry()
                    {
                        State = fileState,
                        Url = url,
                        CommitDate = commitDate
                    });
                }
            }
        }

        // hook is not null
        protected virtual IEnumerable<GithubFileEntry> GetFileUrls(GithubHook hook)
        {

            Dictionary<string, GithubFileEntry> fileEntries = new Dictionary<string, GithubFileEntry>();
            //hook.Commits.OrderBy(c => c.Timestamp)
            foreach (var commit in hook.Commits)
            {                
                ProcessCommitFilesWithState(fileEntries, commit.Added, FileState.Added, commit.Timestamp, hook.Repository);
                ProcessCommitFilesWithState(fileEntries, commit.Modified, FileState.Modified, commit.Timestamp, hook.Repository);
                ProcessCommitFilesWithState(fileEntries, commit.Removed, FileState.Removed, commit.Timestamp, hook.Repository);                
            }            

            return fileEntries.Where(entry => entry.Value.State != FileState.Removed).Select(entry => entry.Value);
        }

        protected GithubHook GetHookFromJson(string hookJson)
        {
            GithubHook hook = null;
            try
            {
                hook = JsonConvert.DeserializeObject<GithubHook>(hookJson);
            }
            catch (Exception)
            {
                return null;
            }

            if (hook.Commits == null || hook.Commits.Count == 0
                || hook.Repository == null || string.IsNullOrEmpty(hook.Repository.Full_Name))
            {
                return null;
            }

            return hook;
        }

        public async virtual Task ConsumeHookAsync(string hookJson)
        {
            if (salesHandlerFactory == null)
            {
                return;
            }

            GithubHook hook = GetHookFromJson(hookJson);

            if (hook == null)
            {
                return;
            }

            var fileEntries = GetFileUrls(hook);

            List<Task> tasks = new List<Task>();
            List<SalesHandlerBase> handlers = new List<SalesHandlerBase>();
            foreach (var entry in fileEntries)
            {
                SalesHandlerBase saleFileHandler = salesHandlerFactory.GetSalesHandler();
                handlers.Add(saleFileHandler);

                ISaleDataSource saleDataSource = new GithubSaleDataSource(entry, token);
                Task task = saleFileHandler.HandleSaleSourceAsync(saleDataSource);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);

            for (int handlerIndex = 0; handlerIndex < handlers.Count; handlerIndex++)
            {
                handlers[handlerIndex].Dispose();
            }
        }
    }
}
