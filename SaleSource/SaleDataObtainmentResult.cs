using Sales.Storage.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.SaleSource
{
    public class SaleDataObtainmentResult
    {
        public SaleDataDto SaleData { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } 
    }
}
