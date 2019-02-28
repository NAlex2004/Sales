using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.SalesEntity.Entity
{
    public class Sale
    {
        [Key]
        public int SaleId { get; set; }
        [Required]
        public int SourceFileId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public DateTime SaleDate { get; set; }
        [Required]
        public decimal TotalSum { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("SourceFileId")]
        public virtual SourceFile SourceFile { get; set; }
    }
}
