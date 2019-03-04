using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Storage.Management
{
    public class SaleManagementResult
    {
        public bool Succeeded { get; set; }
        public string FileName { get; set; }
        public string ErrorMessage { get; set; }        
    }
}
