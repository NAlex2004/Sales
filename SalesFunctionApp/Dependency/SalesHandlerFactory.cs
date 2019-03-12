using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.SaleSource;
using Sales.SaleSource.Factory;
using Unity;

namespace Sales.SalesFunctionApp.Dependency
{
    public class SalesHandlerFactory : ISalesHandlerFactory
    {
        private IUnityContainer container;

        public SalesHandlerFactory(IUnityContainer container)
        {
            this.container = container;
        }

        public SalesHandlerBase GetSalesHandler()
        {
            return container.Resolve<SalesHandlerBase>();
        }
    }
}
