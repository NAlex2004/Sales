using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Storage.DTO
{
    public class SaleDataDto
    {
        public DateTime FileDate { get; set; }
        public string SourceFileName { get; set; }
        public IList<SaleDto> Sales { get; set; } = new List<SaleDto>();
    }
}
