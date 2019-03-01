using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sales.SalesEntity.Entity
{
    public class SourceFile
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(500)]
        public string FileName { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}
