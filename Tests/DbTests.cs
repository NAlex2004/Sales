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

                int oldSalesCount = unitOfWork.Sales.Get().Count();
                int oldCustomersCount = unitOfWork.Customers.Get().Count();
                int oldProductCount = unitOfWork.Products.Get().Count();

                int res = unitOfWork.SaveChanges();

                int salesCount = unitOfWork.Sales.Get().Count();
                int customersCount = unitOfWork.Customers.Get().Count();
                int productCount = unitOfWork.Products.Get().Count();

                Assert.AreEqual(2 + oldSalesCount, salesCount);
                Assert.AreEqual(1 + oldCustomersCount, customersCount);
                Assert.AreEqual(2 + oldProductCount, productCount);
            }
        }

        [TestMethod]
        public void AddingSameProduct_AddedOnce()
        {
            using (var unitOfWork = new SalesDbUnitOfWork(new SalesDbContext()))
            {
                unitOfWork.Products.Delete(p => true);
                unitOfWork.SaveChanges();

                Product product1 = new Product()
                {
                    ProductName = "Product 1"
                };
                Product product2 = new Product()
                {
                    ProductName = "Product 1"
                };
                Product product3 = new Product()
                {
                    ProductName = "Product 1"
                };
                Product product4 = new Product()
                {
                    ProductName = "Product 1"
                };

                var added = unitOfWork.Products.AddRange(new[] { product1, product2, product3 });
                int res = unitOfWork.SaveChanges();

                unitOfWork.Products.Add(product4);

                res = unitOfWork.SaveChanges();
                int count = unitOfWork.Products.Get().Count();

                Assert.AreEqual(1, count);
            }
        }

        [TestMethod]
        public void SaveChanges_Fails_NothingSaved()
        {
            SourceFile sourceFile1 = new SourceFile()
            {
                FileName = "shit"
            };

            SourceFile sourceFile2 = new SourceFile()
            {
                FileName = "shit 2"
            };

            SourceFile sourceFile3 = new SourceFile()
            {
                FileName = "shit"
            };

            using (var unitOfWork = new SalesDbUnitOfWork(new SalesDbContext()))
            {
                unitOfWork.SourceFiles.Delete(f => true);
                unitOfWork.SaveChanges();

                unitOfWork.SourceFiles.Add(sourceFile1);
                unitOfWork.SourceFiles.Add(sourceFile2);
                unitOfWork.SourceFiles.Add(sourceFile3);

                try
                {
                    unitOfWork.SaveChanges();
                    Assert.Fail();
                }
                catch (Exception)
                {
                    int count = unitOfWork.SourceFiles.Get().Count();
                    Assert.AreEqual(0, count);
                }
            }
        }
    }
}
