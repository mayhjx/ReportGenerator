using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Models;

namespace ReportGenerator.Data
{
    public class ProjectParametersContext : DbContext
    {
        public ProjectParametersContext (DbContextOptions<ProjectParametersContext> options)
            : base(options)
        {
        }

        public DbSet<ReportGenerator.Models.ProjectParameter> ProjectParameter { get; set; }
    }
}
