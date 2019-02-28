using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sales.DAL.Database;
using Sales.DAL.Interfaces;
using Sales.SalesEntity.Entity;
using Sales.Storage.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class DbTests
    {
        [TestMethod]
        public void AddNewSaleWithNewProduct_AddsProduct()
        {
            using (ISalesUnitOfWork unitOfWork = new SalesDbUnitOfWork(new SalesDbContext()))
            {                
                SourceFile sourceFile = new SourceFile()
                {
                    FileName = "shit"
                };

                Customer customer1 = new Customer()
                {
                    CustomerName = "Customer"
                };
                Product product1 = new Product()
                {
                    ProductName = "Product"
                };

                Sale sale1 = new Sale()
                {
                    SourceFile = sourceFile,
                    Customer = customer1,
                    Product = product1,
                    SaleDate = DateTime.UtcNow,
                    TotalSum = 10
                };

                Customer customer2 = new Customer()
                {
                    CustomerName = "Customer 2"
                };
                Product product2 = new Product()
                {
                    ProductName = "Product 2"
                };

                Sale sale2 = new Sale()
                {
                    SourceFile = sourceFile,
                    Customer = customer1,
                    Product = product2,
                    SaleDate = DateTime.UtcNow,
                    TotalSum = 122
                };

                var added = unitOfWork.Sales.AddRange(new Sale[] { sale1, sale2 });
                int res = unitOfWork.SaveChanges();

                int salesCount = unitOfWork.Sales.Get().Count();
                int customersCount = unitOfWork.Customers.Get().Count();
                int productCount = unitOfWork.Products.Get().Count();

                Assert.AreEqual(2, salesCount);
                Assert.AreEqual(1, customersCount);
                Assert.AreEqual(2, productCount);
            }
        }
    }
}
