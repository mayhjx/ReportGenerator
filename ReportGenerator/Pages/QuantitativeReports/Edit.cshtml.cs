using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReportGenerator.Data;
using ReportGenerator.Models;
using System.Threading.Tasks;

namespace ReportGenerator.Pages.QuantitativeReports
{
    public class EditModel : PageModel
    {
        private readonly ReportContext _context;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ReportContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
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
                i => i.ProtocalID, i => i.LastInvestigationDate, i => i.Purpose,
                i => i.Item, i => i.TargetReagentLot, i => i.MatchReagentLot,
                i => i.StartTestDate, i => i.EndTestDate, i => i.EvaluationDate,
                i => i.Technician, i => i.Investigator, i => i.InvestigationDate,
                i => i.Approver, i => i.ApprovalDate))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
