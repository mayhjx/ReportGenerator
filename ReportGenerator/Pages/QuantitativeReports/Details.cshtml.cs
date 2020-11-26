using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Data;
using ReportGenerator.Models;
using System.Threading.Tasks;

namespace ReportGenerator.Pages.QuantitativeReports
{
    public class DetailsModel : PageModel
    {
        private readonly ReportContext _context;

        public DetailsModel(ReportContext context)
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
