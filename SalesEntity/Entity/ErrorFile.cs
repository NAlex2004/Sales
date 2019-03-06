using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.SalesEntity.Entity
{
    public class ErrorFile
    {
        [Key]
        public int Id { get; set; }
        [Required]        
        [MaxLength(500)]
        [Index("idx_Errors", IsUnique = true, Order = 1)]
        public string FileName { get; set; }
        [Required]
        [MaxLength(2500)]
        [Index("idx_Errors", IsUnique = true, Order = 2)]
        public string ErrorDescription { get; set; }
    }
}
