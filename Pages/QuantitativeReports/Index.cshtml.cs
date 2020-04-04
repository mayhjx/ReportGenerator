using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Data;
using ReportGenerator.Models;

namespace ReportGenerator.Pages.QuantitativeReports
{
    public class IndexModel : PageModel
    {
        private readonly ReportGenerator.Data.ReportContext _context;

        public IndexModel(ReportGenerator.Data.ReportContext context)
        {
            _context = context;
        }

        public IList<Report> Report { get;set; }

        public async Task OnGetAsync()
        {
            Report = await _context.Report.ToListAsync();
        }
    }
}
