using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Sales.SaleSource.Github;
using Sales.SaleSource;
using Sales.Storage.DTO;
using Sales.Storage.Management;
using Sales.DAL.Interfaces;
using Sales.DAL.Database;
using Sales.SaleSource.Factory;
using Sales.SaleSource.Validation;
using Sales.SalesEntity.Entity;

namespace Tests
{
    public static class DependencyContainer
    {
        private static IUnityContainer container;

        public static IUnityContainer Container
        {
            get
            {
                if (container == null)
                {
                    container = new UnityContainer();
                    CreateRegistrations();
                }

                return container;
            }
        }

        private static void CreateRegistrations()
        {
            container.RegisterType<SalesDbContext, TestDbContext>();
            container.RegisterType<ISalesUnitOfWork, SalesDbUnitOfWork>();// Invoke.Constructor(Resolve.Parameter()));
            container.RegisterType<ISalesDataManager, SaleDbDataManager>();
            container.RegisterType<SalesHandlerBase, GithubSalesHandler>();
            container.RegisterType<ISaleDataSource, GithubSaleDataSource>();
            container.RegisterType<IHookConsumer, GithubHookConsumer>();
            
        }
    }
}
