using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAlex.SalesEntity.Entity
{
    public class ErrorFile
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FileName { get; set; }
    }
}
