using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Data;
using ReportGenerator.Models;
using Microsoft.AspNetCore.Authorization;

namespace ReportGenerator.Pages.ProjectParameters
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ReportGenerator.Data.ProjectParametersContext _context;

        public EditModel(ReportGenerator.Data.ProjectParametersContext context)
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

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ProjectParameter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectParameterExists(ProjectParameter.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProjectParameterExists(int id)
        {
            return _context.ProjectParameter.Any(e => e.ID == id);
        }
    }
}
