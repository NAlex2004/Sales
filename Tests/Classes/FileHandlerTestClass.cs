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
    public class FileHandlerTestClass : GithubSaleFileHandler
    {
        public FileHandlerTestClass(ISalesDataManager salesDataManager, string token) : base(salesDataManager, token)
        {
        }

        public FileHandlerTestClass(string token) : base(new SaleDbDataManager(), token)
        {
        }

        protected override Task<SaleDataDto> GetSalesFromGithub(string url)
        {
            return base.GetSalesFromGithub(url);
        }

        public SaleDataDto GetSalesFromGithubSync(string url)
        {
            return GetSalesFromGithub(url).GetAwaiter().GetResult();
        }
    }
}
