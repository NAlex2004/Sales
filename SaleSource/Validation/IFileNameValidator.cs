using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.SaleSource.Validation
{
    public interface IFileNameValidator
    {
        bool Validate(string fileName);
    }
}
