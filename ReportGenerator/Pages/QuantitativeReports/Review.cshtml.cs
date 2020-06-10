using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Data;
using ReportGenerator.Models;
using System.Threading.Tasks;

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
                i => i.Status, i => i.Approver, i => i.ApprovalDate))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }

        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //// more details see https://aka.ms/RazorPagesCRUD.
        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }

        //    _context.Attach(Report).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ReportExists(Report.ID))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return RedirectToPage("./Index");
        //}

        //private bool ReportExists(int id)
        //{
        //    return _context.Report.Any(e => e.ID == id);
        //}
    }
}
