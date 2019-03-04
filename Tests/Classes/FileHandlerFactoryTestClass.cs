using Sales.SaleSource;
using Sales.SaleSource.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace Tests.Classes
{
    public class FileHandlerFactoryTestClass : ISalesHandlerFactory
    {
        bool generateEmpty;
        string token;

        public FileHandlerFactoryTestClass(bool generateEmptyHandlers = true)
        {
            generateEmpty = generateEmptyHandlers;
        }

        private string Token
        {
            get
            {
                if (string.IsNullOrEmpty(token))
                {
                    //token = ConfigurationManager.AppSettings["token"];
                    token = File.ReadAllText("../../Data/token.txt");
                }

                return token;
            }
        }

        public SalesHandlerBase GetSalesHandler()
        {
            if (generateEmpty)
            {
                return new EmptySaleFileHandlerTestClass();
            }

            return new FileHandlerTestClass(Token);
        }
    }
}
