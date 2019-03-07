using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sales.SalesWebWatcher.Models
{
    public class SaleViewModel
    {
        public DateTime SaleDate { get; set; }
        public decimal TotalSum { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual string ProductName { get; set; }
        public virtual string SourceFileName { get; set; }
    }
}