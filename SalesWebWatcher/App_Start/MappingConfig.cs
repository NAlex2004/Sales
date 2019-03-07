using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExpressMapper;
using ExpressMapper.Extensions;
using Sales.SalesWebWatcher.Models;
using Sales.SalesEntity.Entity;

namespace Sales.SalesWebWatcher
{
    public class MappingConfig
    {
        public static void CreateMappings()
        {
            Mapper.Register<Customer, CustomerViewModel>();
            Mapper.Register<Product, ProductViewModel>();
            Mapper.Register<ErrorFile, ErrorViewModel>();
            Mapper.Register<SourceFile, SourceFileViewModel>();
            Mapper.Register<Sale, SaleViewModel>()
                .Member(dest => dest.ProductName, src => src.Product.ProductName)
                .Member(dest => dest.CustomerName, src => src.Customer.CustomerName)
                .Member(dest => dest.SourceFileName, src => src.SourceFile.FileName);
        }
    }
}