using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.SalesEntity.Entity
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(500)]
        public string CustomerName { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}
