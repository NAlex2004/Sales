using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Storage.Validation
{
    public class FileNameValidationResult
    {
        public bool IsValid { get; set; }
        public string ManagerInitials { get; set; }
        public DateTime Date { get; set; }
    }
}
