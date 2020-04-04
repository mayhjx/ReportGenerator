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
    public class IndexModel : PageModel
    {
        private readonly ReportGenerator.Data.ProjectParametersContext _context;

        public IndexModel(ReportGenerator.Data.ProjectParametersContext context)
        {
            _context = context;
        }

        public IList<ProjectParameter> ProjectParameter { get;set; }

        public async Task OnGetAsync()
        {
            ProjectParameter = await _context.ProjectParameter.ToListAsync();
        }
    }
}
