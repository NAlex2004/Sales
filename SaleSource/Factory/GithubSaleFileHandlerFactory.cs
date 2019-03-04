using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.SaleSource;
using Sales.Storage.Management;

namespace Sales.SaleSource.Factory
{
    public class GithubSaleFileHandlerFactory : ISaleFileHandlerFactory
    {
        private string token;

        public GithubSaleFileHandlerFactory(string githubToken)
        {
            token = githubToken;
        }

        public SaleFileHandlerBase GetSaleFileHandler()
        {
            return new GithubSaleFileHandler(new SaleDbDataManager(), token);
        }
    }
}
