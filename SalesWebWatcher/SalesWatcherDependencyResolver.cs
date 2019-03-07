using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sales.DAL;
using Sales.DAL.Database;
using Sales.DAL.Interfaces;
using Sales.SalesEntity.Entity;
using Sales.SalesWebWatcher.Controllers;
using Sales.SalesWebWatcher.BL;

namespace Sales.SalesWebWatcher
{
    public class SalesWatcherDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(HomeController))
            {
                return new HomeController(new DataManager(new SalesDbUnitOfWork(new SalesDbContext())));
            }

            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }
    }
}