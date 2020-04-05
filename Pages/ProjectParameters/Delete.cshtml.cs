using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Data;
using ReportGenerator.Models;
using Microsoft.AspNetCore.Authorization;

namespace ReportGenerator.Pages.ProjectParameters
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ReportGenerator.Data.ProjectParametersContext _context;

        public DeleteModel(ReportGenerator.Data.ProjectParametersContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProjectParameter ProjectParameter { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProjectParameter = await _context.ProjectParameter.FirstOrDefaultAsync(m => m.ID == id);

            if (ProjectParameter == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProjectParameter = await _context.ProjectParameter.FindAsync(id);

            if (ProjectParameter != null)
            {
                _context.ProjectParameter.Remove(ProjectParameter);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
