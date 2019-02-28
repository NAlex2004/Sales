﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.DTO
{
    public class SaleDto
    {
        public DateTime SaleDate { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public decimal TotalSum { get; set; }
    }
}
