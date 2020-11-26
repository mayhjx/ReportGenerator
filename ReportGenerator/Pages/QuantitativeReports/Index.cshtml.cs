using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Data;
using ReportGenerator.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportGenerator.Pages.QuantitativeReports
{
    public class IndexModel : PageModel
    {
        private readonly ReportContext _context;

        public IndexModel(ReportContext context)
        {
            _context = context;
        }

        public IList<Report> Report { get; set; }

        public async Task OnGetAsync()
        {
            var reports = from p in _context.Report
                          select p;

            Report = await reports.AsNoTracking().OrderByDescending(t => t.EvaluationDate).ToListAsync();
        }
    }
}
