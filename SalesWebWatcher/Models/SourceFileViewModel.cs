﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sales.SalesWebWatcher.Models
{
    public class SourceFileViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime FileDate { get; set; }
    }
}