using Microsoft.EntityFrameworkCore;
using ReportGenerator.Models;

namespace ReportGenerator.Data
{
    public class ProjectParametersContext : DbContext
    {
        public ProjectParametersContext(DbContextOptions<ProjectParametersContext> options)
            : base(options)
        {
        }

        public DbSet<ProjectParameter> ProjectParameter { get; set; }
    }
}
