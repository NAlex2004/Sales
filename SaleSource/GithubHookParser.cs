using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sales.SaleSource.Github;

namespace Sales.SaleSource
{
    public class GithubHookParser
    {
        private readonly string GITHUB_URL = "https://api.github.com/repos/";
        protected Func<string, bool> fileNameValidator;

        public GithubHookParser(Func<string, bool> fileNameValidator = null)
        {
            this.fileNameValidator = fileNameValidator;                        
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

        public IEnumerable<GithubFileEntry> GetFileEntriesFromHook(string hookJson)
        {
            GithubHook hook = GetHookFromJson(hookJson);

            if (hook == null)
            {
                return new GithubFileEntry[0];
            }

            var fileEntries = GetFileUrls(hook);

            return fileEntries;
        }
    }
}
