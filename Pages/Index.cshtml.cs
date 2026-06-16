using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SatraWebApplication.Data;
using SatraWebApplication.Model;
using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace SatraWebApplication.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _context;

        public IndexModel(ApplicationDBContext context)
        {
            _context = context;
        }

        public string ReportDate;
        public ChartDataModel ChartModel { get; set; }
        public ChartDataModel ChartModelLine { get; set; }

        //public async Task OnGetAsync()
        //{
        //    var data =  _context.WebSiteUpTimes
        //        .GroupBy(m => m.Result)
        //        .Select(x => new DataPoint
        //        {
        //            Label = x.Key,
        //            Value = x.Count()
        //        })
        //        .ToList();

        //    Model = new ChartDataModel
        //    {
        //        Data = data
        //    };
        //}

        //        var data = @Html.Raw(JsonConvert.SerializeObject(Model.Data));

        //        var ctx = document.getElementById('myChart').getContext('2d');
        //        var chart = new Chart(ctx, {

        //            type: 'line',
        //            data: {

        //                labels: data.map(x => x.Label),
        //                datasets: [{
        //            label: 'My dataset',
        //            backgroundColor: 'rgb(255, 99, 132)',
        //            borderColor: 'rgb(255, 99, 132)',
        //            data: data.map(x => x.Value)
        //                }]
        //    },
        //    options: { }
        //});
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        public IList<WebSiteUpTime> websites { get; set; }
        public IList<WebSite> websitesUpTime { get; set; }
        public IList<WebSite> websitesUpTime100 { get; set; }

        public void OnGet()
        {
            //User.IsInRole("Admin")
            //IList<WebSiteUpTime> list= _context.WebSite.Include(t => t.WebSiteUpTimes).ToList()[0].WebSiteUpTimes.ToList();
            //_context.WebSite
            double totlal = _context.WebSite.ToList().Count * 1.0;
            DateTime lastDate = _context.WebSiteUpTime
                .GroupBy(m => m.ResultGroup)
                .Select(x => x.Max(y => y.ResultDate))
                .FirstOrDefault().AddMilliseconds(-1);
            websites = new List<WebSiteUpTime>();
            websitesUpTime = new List<WebSite>();
            websitesUpTime100 = new List<WebSite>();
            if (!string.IsNullOrEmpty(SearchString))
            {
                websites = _context.WebSiteUpTime.Where(s => s.URL.Contains(SearchString) && s.ResultDate>=lastDate).ToList();
                if(SearchString!="100")
                    websitesUpTime = _context.WebSite.Include(t=>t.WebSiteUpTimes).Where(s => s.URL.Contains(SearchString) ).ToList();
                else
                    websitesUpTime = _context.WebSite.Include(t => t.WebSiteUpTimes).Where(s => _context.IsUp100(s.ID)==true).ToList();
                
               
            }
            var data = _context.WebSiteUpTime
                .Where(w=>w.ResultDate>=lastDate)
                .GroupBy(m => m.ResultGroup)
                .Select(x => new DataPoint
                {
                    Label = x.Key,
                    PercentValue = Math.Round((x.Count()/ totlal) *100,2),
                    Value= x.Count()//,                 

                }
                
                
                )
                .ToList();
            //var result = _context.WebSiteUpTime.GroupBy(t => new { t.ResultDate, t.ResultGroup })
            //        .Where(c => c.Key.ResultGroup == "فعال").Select(g => new { ResultDate = g.Key.ResultDate, TotalCount = g.Count() }).OrderBy(o => o.ResultDate);


            //ChartModelLine = new ChartDataModel
            //{
            //    Data = result.Select(x => new DataPoint
            //    {
            //        Label = GetPC(x.ResultDate),
            //        Value = x.TotalCount
            //    }).ToList()
            //};


            ChartModel = new ChartDataModel
            {
                Data = data
            };
            PersianCalendar pc = new PersianCalendar();
            
            
            ReportDate = string.Format("{0}/{1:d2}/{2:d2} {3:d2}:{4:d2}:{5:d2}",
                      
                      pc.GetYear(lastDate),
                      pc.GetMonth(lastDate),                     
                      pc.GetDayOfMonth(lastDate),
                      pc.GetHour(lastDate),
                      pc.GetMinute(lastDate),
                      pc.GetSecond(lastDate));
        }
        private static string GetPC(DateTime d)
        {
            PersianCalendar pc = new PersianCalendar();
            return  string.Format("{0}/{1:d2}/{2:d2} {3:d2}:{4:d2}:{5:d2}",

                      pc.GetYear(d),
                      pc.GetMonth(d),
                      pc.GetDayOfMonth(d),
                      pc.GetHour(d),
                      pc.GetMinute(d),
                      pc.GetSecond(d));

        }
    }
}
