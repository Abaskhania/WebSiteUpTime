using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SatraWebApplication.Data;
using SatraWebApplication.Model;

namespace SatraWebApplication.Pages
{
    [Authorize]
    public class WSResultModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public List<WebSiteUpTime>? siteList;
        public List<SelectListItem> ResponseReason { get; set; } = new();
        [BindProperty]
        public string SelectedResult { get; set; }
        public string ResultFilter { get; set; }
        public WSResultModel(ApplicationDBContext context)
        {
            _context = context;
        }
        public void OnGet(string result)
        {
            


            ResultFilter = result;

            DateTime lastDate = _context.WebSiteUpTime
                .GroupBy(m => m.ResultGroup)
                .Select(x => x.Max(y => y.ResultDate))
                .FirstOrDefault().AddMilliseconds(-1);
           

            var likeSearch = $"%{result}%";
            if (result == "OK")
                likeSearch = "OK";
            siteList = _context.WebSiteUpTime
            .Where(ws => ws.ResultDate>=lastDate && EF.Functions.Like( ws.Result , likeSearch)).OrderByDescending(s=>s.ResultDate).ToList();

            ResponseReason=_context.WebSiteUpTime
               .Where(ws => ws.ResultDate >= lastDate).Select(t=>t.Result).Distinct().Select(t => new SelectListItem() { Text = t, Value = t }).ToList();
        }
    }
}
