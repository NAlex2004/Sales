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
using System.Configuration;

namespace Sales.SalesFunctionApp.Dependency
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
            container.RegisterFactory<GithubHookParser>(cnt => new GithubHookParser(fileName => FileNameValidator.Validate(fileName)));
            container.RegisterType<ISalesUnitOfWork, SalesDbUnitOfWork>();
            container.RegisterType<ISalesDataManager, SaleDbDataManager>();
            container.RegisterType<SalesHandlerBase, GithubSalesHandler>();
            container.RegisterType<ISaleDataSource, GithubSaleDataSource>();
            container.RegisterType<ISalesHandlerFactory, SalesHandlerFactory>(Invoke.Constructor(Container));
        }
    }
}
