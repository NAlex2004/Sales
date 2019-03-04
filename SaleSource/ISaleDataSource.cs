using Sales.Storage.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.SaleSource
{
    public interface ISaleDataSource
    {
        Task<SaleDataDto> GetSaleDataAsync();
    }
}
