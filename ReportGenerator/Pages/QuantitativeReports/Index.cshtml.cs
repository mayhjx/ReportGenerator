using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Data;
using ReportGenerator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ReportGenerator.Pages.QuantitativeReports
{
    public class IndexModel : PageModel
    {
        private readonly ReportContext _context;

        public IndexModel(ReportContext context)
        {
            _context = context;
        }

        public IList<Report> Report { get;set; }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public SelectList Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReportStatus { get; set; }

        public async Task OnGetAsync()
        {
            // 查询报告状态和比对项目名称
            IQueryable<string> StatusQuery = from r in _context.Report
                                             orderby r.Status
                                             select r.Status;

            var reports = from p in _context.Report
                          select p;

            if (!string.IsNullOrEmpty(SearchString))
            {
                reports = reports.Where(s => s.Item.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(ReportStatus))
            {
                reports = reports.Where(x => x.Status == ReportStatus);
            }
            Status = new SelectList(await StatusQuery.Distinct().ToListAsync());
            Report = await reports.ToListAsync();
        }
    }
}
