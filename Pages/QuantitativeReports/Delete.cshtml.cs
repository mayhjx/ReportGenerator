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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;

namespace ReportGenerator.Pages.QuantitativeReports
{
    public class DeleteModel : PageModel
    {
        private readonly ReportContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(ReportContext context, 
            IWebHostEnvironment webHostEnvironment,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
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

            // 删除报告时需验证用户
            // 删除已审核报告需要管理员权限
            // 删除待审核报告无需权限

            // 获取当前用户
            IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null && Report.Status == "已审核")
            {
                return Forbid();
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
