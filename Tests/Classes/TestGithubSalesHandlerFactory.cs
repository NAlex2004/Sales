using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;
using Sales.SaleSource.Factory;
using Sales.SaleSource;

namespace Tests.Classes
{
    public class TestGithubSalesHandlerFactory : ISalesHandlerFactory
    {
        private IUnityContainer container;

        public TestGithubSalesHandlerFactory(IUnityContainer container)
        {
            this.container = container;
        }

        public SalesHandlerBase GetSalesHandler()
        {
            return container.Resolve<SalesHandlerBase>();
        }
    }
}
