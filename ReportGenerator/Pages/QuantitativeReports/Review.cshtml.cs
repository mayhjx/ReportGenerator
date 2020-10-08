using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Data;
using ReportGenerator.Models;

namespace ReportGenerator.Pages.QuantitativeReports
{
    [Authorize]
    public class ReviewModel : PageModel
    {
        private readonly ReportContext _context;

        public ReviewModel(ReportContext context)
        {
            _context = context;
        }

        [BindProperty]
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

            if (Report.Status == "已审核")
            {
                return RedirectToPage("Details", new { id });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var reportToUpdate = await _context.Report.FindAsync(id);

            if (reportToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Report>(
                reportToUpdate,
                "Report",
                i => i.Approver, i => i.ApprovalDate))
            {
                reportToUpdate.Status = "已审核";
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
