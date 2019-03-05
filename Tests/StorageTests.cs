using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sales.SaleSource.Validation;
using ExpressMapper;
using ExpressMapper.Extensions;
using Sales.SalesEntity.Entity;
using Sales.Storage.DTO;
using Sales.Storage.Management;

namespace Tests
{
    [TestClass]
    public class StorageTests
    {
        private Dictionary<string, bool> fileNamesSource = new Dictionary<string, bool>();

        public StorageTests()
        {
            fileNamesSource.Add("", false);            
            fileNamesSource.Add("asdsd", false);
            fileNamesSource.Add("asfde_121212.ll", false);
            fileNamesSource.Add("AlNaz_18121980.json", true);
            fileNamesSource.Add("AlNaz_33121980.json", false);
            fileNamesSource.Add("Iviva_01022010.aa", false);            
        }

        [TestMethod]        
        public void FileNameValidation_Correct()
        {            
            foreach (var entry in fileNamesSource)
            {
                var validationResult = FileNameValidator.Validate(entry.Key);

                Assert.AreEqual(entry.Value, validationResult);
            }
        }

        [TestMethod]
        public void MappingsTest()
        {
            SaleDto saleDto = new SaleDto()
            {
                CustomerName = "Customer",
                ProductName = "Product",
                SaleDate = DateTime.Now,
                TotalSum = 111
            };

            Sale sale = SaleDbDataManager.Mappings.GetMapper().Map<SaleDto, Sale>(saleDto);
            SaleDto mappedSale = SaleDbDataManager.Mappings.GetMapper().Map<Sale, SaleDto>(sale);

            Assert.AreEqual(sale.Customer.CustomerName, saleDto.CustomerName);
            Assert.AreEqual(sale.Product.ProductName, saleDto.ProductName);
            Assert.AreEqual(sale.Customer.CustomerName, mappedSale.CustomerName);
            Assert.AreEqual(sale.Product.ProductName, mappedSale.ProductName);
        }
    }
}
