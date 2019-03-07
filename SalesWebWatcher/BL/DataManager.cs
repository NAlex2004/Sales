using Sales.DAL.Interfaces;
using Sales.SalesWebWatcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sales.SalesEntity.Entity;
using Sales.DAL.Database;
using System.Linq.Expressions;
using ExpressMapper;

namespace Sales.SalesWebWatcher.BL
{
    public class DataManager : IDisposable
    {
        private ISalesUnitOfWork unitOfWork;

        public DataManager(ISalesUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException();
        }

        public IEnumerable<ProductViewModel> GetProducts()
        {
            var products = unitOfWork.Products.Get().ToList();
            var productsModel = Mapper.Map<List<Product>, List<ProductViewModel>>(products);
            return productsModel;
        }

        public IEnumerable<CustomerViewModel> GetCustomers()
        {
            var customers = unitOfWork.Customers.Get().ToList();
            var customersModel = Mapper.Map<List<Customer>, List<CustomerViewModel>>(customers);
            return customersModel;
        }

        public IEnumerable<SourceFileViewModel> GetSourceFiles()
        {
            var sourceFiles = unitOfWork.SourceFiles.Get().ToList();
            var sourceFilesModel = Mapper.Map<List<SourceFile>, List<SourceFileViewModel>>(sourceFiles);
            return sourceFilesModel;
        }

        public IEnumerable<ErrorViewModel> GetErrors()
        {
            var errors = unitOfWork.ErrorFiles.Get().ToList();
            var errorModel = Mapper.Map<List<ErrorFile>, List<ErrorViewModel>>(errors);
            return errorModel;
        }

        public IEnumerable<SaleViewModel> GetSales()
        {
            var sales = unitOfWork.Sales.Get()
                .OrderBy(s => s.SaleDate)
                .ThenBy(s => s.SourceFile.FileName)
                .ThenBy(s => s.Customer.CustomerName)
                .ThenBy(s => s.Product.ProductName)
                .ToList();
            var salesModel = Mapper.Map<List<Sale>, List<SaleViewModel>>(sales);
            return salesModel;
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    unitOfWork.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);         
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}