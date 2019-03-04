using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sales.DAL.Database;
using Sales.DAL.Interfaces;
using Sales.SalesEntity.Entity;
using Sales.Storage.DTO;
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
        private SaleDataDto GetSaleData(int additionalSum = 0)
        {
            SaleDataDto saleData = new SaleDataDto()
            {
                SourceFileName = "AlNaz_18121980.json"
            };

            List<SaleDto> saleDtos = new List<SaleDto>();

            for (int i = 0; i < 10; i++)
            {
                SaleDto saleDto = new SaleDto()
                {
                    SaleDate = new DateTime(2018, i + 1, i + 1),
                    CustomerName = $"Customer #{i + 1}",
                    ProductName = $"Product #{i + 1}",
                    TotalSum = (i + 10) * 10
                };

                SaleDto saleDto2 = new SaleDto()
                {
                    SaleDate = new DateTime(2018, i + 1, i + 1),
                    CustomerName = $"Customer #{i + 1}",
                    ProductName = $"Product #{i + 1}",
                    TotalSum = (i + 10) * 10 + additionalSum
                };

                saleDtos.Add(saleDto);
                saleDtos.Add(saleDto2);
            }

            saleData.Sales = saleDtos;
            return saleData;
        }

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
        public void When_SaveChanges_Fails_NothingSaved()
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

        [TestMethod]
        public void SaleDbDataManager_AddData_CorrectlyAddsDuplicates_AndSameFile()
        {
            SaleDataDto saleDataDto = GetSaleData();
            var groupedSales = saleDataDto.Sales
                .GroupBy(sale => new { sale.CustomerName, sale.ProductName, sale.SaleDate },
                (key, sales) => new SaleDto()
                {
                    CustomerName = key.CustomerName,
                    ProductName = key.ProductName,
                    SaleDate = key.SaleDate,
                    TotalSum = sales.Sum(s => s.TotalSum)
                });
            int customersCount = saleDataDto.Sales.Select(s => s.CustomerName).Distinct().Count();
            int productsCount = saleDataDto.Sales.Select(s => s.ProductName).Distinct().Count();
            int salesCount = groupedSales.Count();

            using (ISalesUnitOfWork unitOfWork = new SalesDbUnitOfWork(new SalesDbContext()))
            using (SaleDbDataManager manager = new SaleDbDataManager(unitOfWork))
            {
                int initialCustomersCount = unitOfWork.Customers.Get().Count();
                int initialProductsCount = unitOfWork.Products.Get().Count();
                int initialSalesCount = unitOfWork.Sales.Get().Count();

                var res = manager.AddOrUpdateSaleDataAsync(saleDataDto).GetAwaiter().GetResult();

                int savedSalesCount = unitOfWork.Sales.Get().Count();
                int savedCustomersCount = unitOfWork.Customers.Get().Count();
                int savedProductsCount = unitOfWork.Products.Get().Count();

                Assert.IsTrue(res.Succeeded);
                Assert.AreEqual(customersCount + initialCustomersCount, savedCustomersCount);
                Assert.AreEqual(productsCount + initialProductsCount, savedProductsCount);
                Assert.AreEqual(salesCount + initialSalesCount, savedSalesCount);

                saleDataDto = GetSaleData(1);
                groupedSales = saleDataDto.Sales
                    .GroupBy(sale => new { sale.CustomerName, sale.ProductName, sale.SaleDate },
                    (key, sales) => new SaleDto()
                    {
                        CustomerName = key.CustomerName,
                        ProductName = key.ProductName,
                        SaleDate = key.SaleDate,
                        TotalSum = sales.Sum(s => s.TotalSum)
                    });

                res = manager.AddOrUpdateSaleDataAsync(saleDataDto).GetAwaiter().GetResult();

                Assert.IsTrue(res.Succeeded);
                savedSalesCount = unitOfWork.Sales.Get().Count();
                savedCustomersCount = unitOfWork.Customers.Get().Count();
                savedProductsCount = unitOfWork.Products.Get().Count();

                Assert.AreEqual(customersCount + initialCustomersCount, savedCustomersCount);
                Assert.AreEqual(productsCount + initialProductsCount, savedProductsCount);
                Assert.AreEqual(salesCount + initialSalesCount, savedSalesCount);
            }
        }
    }
}
