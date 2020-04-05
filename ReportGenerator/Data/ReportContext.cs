﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Models;

namespace ReportGenerator.Data
{
    public class ReportContext : DbContext
    {
        public ReportContext (DbContextOptions<ReportContext> options)
            : base(options)
        {
        }

        public DbSet<ReportGenerator.Models.Report> Report { get; set; }
    }
}
