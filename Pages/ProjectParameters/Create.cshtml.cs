using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReportGenerator.Data;
using ReportGenerator.Models;
using Microsoft.AspNetCore.Authorization;

namespace ReportGenerator.Pages.ProjectParameters
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ReportGenerator.Data.ProjectParametersContext _context;

        public CreateModel(ReportGenerator.Data.ProjectParametersContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ProjectParameter ProjectParameter { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ProjectParameter.Add(ProjectParameter);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
