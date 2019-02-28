using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.SalesEntity.Entity
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public int ProductName { get; set; }        
    }
}
