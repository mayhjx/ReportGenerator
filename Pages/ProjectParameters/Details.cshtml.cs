using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Data;
using ReportGenerator.Models;

namespace ReportGenerator.Pages.ProjectParameters
{
    public class DetailsModel : PageModel
    {
        private readonly ReportGenerator.Data.ProjectParametersContext _context;

        public DetailsModel(ReportGenerator.Data.ProjectParametersContext context)
        {
            _context = context;
        }

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
    }
}
