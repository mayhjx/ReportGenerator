using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReportGenerator.Data;
using ReportGenerator.Models;

namespace ReportGenerator.Pages.QuantitativeReports
{
    public class CreateModel : PageModel
    {
        private readonly ReportGenerator.Data.ReportContext _context;

        public CreateModel(ReportGenerator.Data.ReportContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            //return Page();
            return RedirectToPage("/Upload");
        }

        public IActionResult OnGetGenerate(Report report)
        {
            this.Report = report;
            return Page();
        }

        [BindProperty]
        public Report Report { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Report.Add(Report);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
