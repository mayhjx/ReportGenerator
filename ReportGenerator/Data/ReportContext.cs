using Microsoft.EntityFrameworkCore;
using ReportGenerator.Models;

namespace ReportGenerator.Data
{
    public class ReportContext : DbContext
    {
        public ReportContext(DbContextOptions<ReportContext> options)
            : base(options)
        {
        }

        public DbSet<Report> Report { get; set; }
    }
}
