using System;
using System.Collections.Generic;
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

        protected IEnumerable<string> GetFileUrls(GithubHook hook)
        {
            List<string> urls = new List<string>();


            return urls;
        }

        public async Task ConsumeHookAsync(string hookJson)
        {
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
