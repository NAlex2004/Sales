using ExpressMapper;
using ExpressMapper.Extensions;
using Sales.SalesEntity.Entity;
using Sales.Storage.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Storage.Management
{
    public partial class SaleDbDataManager
    {
        public class Mappings
        {
            static Mappings()
            {
                Mapper.Register<Sale, SaleDto>().Member(dto => dto.CustomerName, src => src.Customer.CustomerName)
                    .Member(dto => dto.ProductName, src => src.Product.ProductName);
                Mapper.Register<SaleDto, Sale>().Function(src => src.Product, dto => new Product() { ProductName = dto.ProductName })
                    .Function(src => src.Customer, dto => new Customer() { CustomerName = dto.CustomerName });
            }

            public static IMappingServiceProvider GetMapper()
            {
                return Mapper.Instance;
            }
        }
    }
}
