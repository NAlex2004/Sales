using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.SaleSource.Github
{
    public class GithubHook
    {
        public IList<GithubCommit> Commits { get; set; }
        public GithubRepository Repository { get; set; }
    }
}
