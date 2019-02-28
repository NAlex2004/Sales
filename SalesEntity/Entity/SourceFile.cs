using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace NAlex.SalesEntity.Entity
{
    public class SourceFile
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FileName { get; set; }
    }
}
