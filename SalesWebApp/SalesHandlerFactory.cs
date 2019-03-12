using Sales.SaleSource;
using Sales.SaleSource.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unity;
using Unity.Resolution;

namespace Sales.SalesWebApp
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