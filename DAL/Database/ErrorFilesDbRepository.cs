﻿using Sales.SalesEntity.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.DAL.Database
{
    public class ErrorFilesDbRepository : GenericDbRepository<ErrorFile>
    {
        public ErrorFilesDbRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
