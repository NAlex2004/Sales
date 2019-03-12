using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.SaleSource;
using Sales.Storage.DTO;
using Sales.Storage.Management;

namespace Tests.Classes
{
    public class FileHandlerTestClass : GithubSalesHandler
    {
        string token;

        public FileHandlerTestClass(ISalesDataManager salesDataManager, string token) : base(salesDataManager)
        {
            this.token = token;
        }
        
        public SaleDataDto GetSalesFromGithubSync(string url)
        {
            ISaleDataSource dataSource = new GithubSaleDataSource(new Sales.SaleSource.Github.GithubFileEntry() { Url = url }, token);
            return dataSource.GetSaleDataAsync().GetAwaiter().GetResult().SaleData;
        }
    }
}
