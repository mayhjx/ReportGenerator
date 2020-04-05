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
    public class DetailsModel : PageModel
    {
        private readonly ReportGenerator.Data.ReportContext _context;

        public DetailsModel(ReportGenerator.Data.ReportContext context)
        {
            _context = context;
        }

        public Report Report { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Report = await _context.Report.FirstOrDefaultAsync(m => m.ID == id);

            if (Report == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
