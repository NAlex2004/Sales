using Sales.Storage.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Storage.Management
{
    public interface ISalesDataManager
    {
        Task<bool> AddOrUpdateSaleDataAsync(SaleDataDto saleData);
    }
}
