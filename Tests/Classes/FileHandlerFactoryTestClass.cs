using Sales.SaleSource;
using Sales.SaleSource.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using Sales.Storage.Management;
using Unity;

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
                    try
                    {
                        token = File.ReadAllText("../../Data/token.txt");
                    }
                    catch
                    {
                        token = "123";
                    }
                    
                }

                return token;
            }
        }

        public SalesHandlerBase GetSalesHandler()
        {
            var dataManager = DependencyContainer.Container.Resolve<ISalesDataManager>();
            if (generateEmpty)
            {                
                return new EmptySaleFileHandlerTestClass(dataManager);
            }

            return new FileHandlerTestClass(dataManager, Token);
        }
    }
}
