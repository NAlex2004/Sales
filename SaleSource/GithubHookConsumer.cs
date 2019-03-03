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

namespace Sales.SaleSource
{
    public class GithubHookConsumer : IHookConsumer
    {
        private const string GITHUB_URL = "https://api.github.com/repos/";
        protected SaleFileHandlerBase saleFileHandler;
        protected Func<string, bool> fileNameValidator;

        public GithubHookConsumer(SaleFileHandlerBase saleFileHandler, Func<string, bool> fileNameValidator = null)
        {
            this.saleFileHandler = saleFileHandler;
            this.fileNameValidator = fileNameValidator;
        }

        protected string MakeGetUrl(string filePath, GithubRepository repository)
        {
            StringBuilder urlBuilder = new StringBuilder(GITHUB_URL);
            urlBuilder.Append(repository.Full_Name).Append("/contents/").Append(filePath);
            return urlBuilder.ToString();
        }        

        private void ProcessCommitFilesWithState(Dictionary<string, FileState> urls, IList<string> fileNames, FileState fileState)
        {
            foreach (string fileName in fileNames)
            {
                if (string.IsNullOrEmpty(fileName)
                    || (fileNameValidator != null && !fileNameValidator(Path.GetFileName(fileName))))
                {                                        
                    continue;                    
                }

                if (urls.ContainsKey(fileName))
                {
                    urls[fileName] = fileState;
                }
                else
                {
                    urls.Add(fileName, fileState);
                }
            }
        }

        protected IEnumerable<string> GetFileUrls(GithubHook hook)
        {
            Dictionary<string, FileState> urls = new Dictionary<string, FileState>();

            //hook.Commits.OrderBy(c => c.Timestamp)
            foreach (var commit in hook.Commits)
            {
                ProcessCommitFilesWithState(urls, commit.Added, FileState.Added);
                ProcessCommitFilesWithState(urls, commit.Modified, FileState.Modified);
                ProcessCommitFilesWithState(urls, commit.Removed, FileState.Removed);
            }

            var result = urls.Where(entry => entry.Value != FileState.Removed).Select(entry => entry.Key);
            return result;
        }

        public async Task ConsumeHookAsync(string hookJson)
        {
            if (saleFileHandler == null)
            {
                return;
            }

            GithubHook hook = null;
            try
            {
                hook = JsonConvert.DeserializeObject<GithubHook>(hookJson);
            }
            catch (JsonSerializationException)
            {
                return;
            }

            if (hook.Commits == null || hook.Commits.Count == 0 
                || hook.Repository == null || string.IsNullOrEmpty(hook.Repository.Full_Name))
            {
                return;
            }

            var fileUrls = GetFileUrls(hook);

            foreach (string url in fileUrls)
            {
                await saleFileHandler.HandleSaleFileAsync(url);                
            }
        }
    }
}
