using Sales.SaleSource;
using Sales.SalesWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;
using Sales.SaleSource.Factory;
using Sales.SaleSource.Validation;

namespace Sales.SalesWebApp
{
    public class SalesDependencyResolver : IDependencyResolver
    {
        public IDependencyScope BeginScope()
        {
            return new SalesDependencyResolver();
        }

        public void Dispose()
        {            
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(SalesController))
            {
                IHookConsumer hookConsumer = new GithubHookConsumer(new GithubSalesHandlerFactory(), "", fileName => FileNameValidator.Validate(fileName));
                return new SalesController(hookConsumer);
            }

            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }
    }
}