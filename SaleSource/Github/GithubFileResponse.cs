using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.SaleSource.Github
{    
    public class GithubFileResponse
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Download_Url { get; set; }
        public string Content { get; set; }
    }
}
