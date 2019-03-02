using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.SaleSource.Github
{
    public class GithubCommit
    {
        public DateTime Timestamp { get; set; }
        public IList<string> Added { get; set; }
        public IList<string> Modified { get; set; }
        public IList<string> Removed { get; set; }
    }
}
