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
        [Index(IsUnique = true)]
        [MaxLength(500)]
        public string FileName { get; set; }
        public string ErrorDescription { get; set; }
    }
}
