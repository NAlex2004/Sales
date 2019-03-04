using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.SaleSource;

namespace Sales.SaleSource.Factory
{
    public interface ISalesHandlerFactory
    {
        SalesHandlerBase GetSalesHandler();
    }
}
