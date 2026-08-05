using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using SatraWebApplication.Data;
using SatraWebApplication.Model;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SatraWebApplication.Pages
{
    [Authorize]
    public class ListWebsiteModel : PageModel
    {
        public List<WebSiteUpTime>? siteList;
        private readonly ApplicationDBContext _context;
        public string? ReportTitle;
        [TempData]
        public string? AlertMessage { get; set; }
        public string? State { get; set; }
        public ChartDataModel Model { get; set; }
        public ChartDataModel ChartModelLine { get; set; }
        public ListWebsiteModel(ApplicationDBContext context)
        {
            _context = context;
        }
        public void OnGet(string state)
        {
            this.AlertMessage = "";

            DateTime lastDate = _context.WebSiteUpTime
                .GroupBy(m => m.ResultGroup)
                .Select(x => x.Max(y => y.ResultDate))
                .FirstOrDefault().AddMilliseconds(-1);

            siteList = _context.WebSiteUpTime
                .Where(ws => ws.ResultGroup == state && ws.ResultDate >= lastDate).OrderBy(r => r.Result).ToList();

            var data = _context.WebSiteUpTime
                     .Where(w => w.ResultDate >= lastDate && w.ResultGroup==state)
                     .GroupBy(m => m.Result)
                     .Select(x => new DataPoint
                     {
                         Label = x.Key,
                         PercentValue =0,
                         Value = x.Count()//,                 

                     })
                     .OrderByDescending(x=>x.Value)
                     .Take(10)
                    .ToList();

            Model = new ChartDataModel
            {
                Data = data
            };

            ReportTitle = " گزارش رسانه های  " + state;
            this.State = state;
            
            {
                var result = _context.WebSiteUpTime
                        .GroupBy(t => new { t.ResultDate, t.ResultGroup })
                        .Where(c => c.Key.ResultGroup == state)
                        .Select(g => new { ResultDate = g.Key.ResultDate, TotalCount = g.Count() })
                        .OrderByDescending(o => o.ResultDate)
                        .Take(30)
                        .OrderBy(o => o.ResultDate);


                ChartModelLine = new ChartDataModel
                {
                    Data = result.Select(x => new DataPoint
                    {
                        Label = GetPC(x.ResultDate),
                        Value = x.TotalCount
                    }).ToList()
                };
            }

        }
        public void OnGetRefresh(int id, string state,string url)
        {
            
            WebSiteUpTime objWebSite = _context.WebSiteUpTime.Where(ws => ws.ID == id).FirstOrDefault()!;
            using (HttpClient c=new HttpClient())
            {
                try
                {
                    //Parallel.ForEach(_context.WebSiteUpTime.Where(t=>t.ResultGroup==state), item =>
                    //{
                        HttpResponseMessage response = c.GetAsync(/*item.URL*/objWebSite.URL).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            objWebSite.ResultGroup = "فعال";
                            objWebSite.Result = "OK";

                        }
                        else
                        {
                            objWebSite.ResultGroup = "غیر فعال";
                            objWebSite.Result = response.ReasonPhrase!;

                        }
                    //_context.SaveChanges();
                    //});
                    //this.AlertMessage = $"بروز رسانی لیست {state} انجام شد. ";
                   
                    
                }
                catch (Exception ex)
                {
                    objWebSite.ResultGroup = "غیر فعال";
                    objWebSite.Result = ex.Message;
                    
                }
                this.AlertMessage = "";
                _context.SaveChanges();
                if (objWebSite.ResultGroup != state)
                    this.AlertMessage = "وضعیت وب سایت " + "[" + objWebSite.URL + "]" + " به " + objWebSite.ResultGroup + " تغییر کرد. ";
                siteList = _context.WebSiteUpTime
                .Where(ws => ws.ResultGroup == state).OrderBy(r => r.Result).ToList();
                ReportTitle = "لیست سایت های " + state;



            }
        }
        private static string GetPC(DateTime d)
        {
            PersianCalendar pc = new PersianCalendar();
            return string.Format("{0}/{1:d2}/{2:d2}",

                      pc.GetYear(d),
                      pc.GetMonth(d),
                      pc.GetDayOfMonth(d),
                      pc.GetHour(d),
                      pc.GetMinute(d),
                      pc.GetSecond(d));

        }
    }
}
