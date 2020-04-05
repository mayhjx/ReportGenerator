using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Data;
using ReportGenerator.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ReportGenerator.Pages.QuantitativeReports
{
    public class DeleteModel : PageModel
    {
        private readonly ReportContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DeleteModel(ReportContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Report = await _context.Report.FindAsync(id);

            if (Report != null)
            {
                // 删除关联的图片
                var path = _webHostEnvironment.WebRootPath + Report.PicturePath;

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                _context.Report.Remove(Report);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
