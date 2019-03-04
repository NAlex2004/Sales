using Sales.SaleSource;
using Sales.SaleSource.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Classes
{
    public class FileHandlerFactoryTestClass : ISaleFileHandlerFactory
    {
        public SaleFileHandlerBase GetSaleFileHandler()
        {
            return new SaleFileHandlerTestClass();
        }
    }
}
