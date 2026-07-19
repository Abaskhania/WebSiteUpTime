using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SatraWebApplication.Data;
using SatraWebApplication.Model;
using System.Net.NetworkInformation;

namespace SatraWebApplication.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class WSStatusController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        
        public string CurrentState { get; set; }
        public WebSite WebsiteInfo { get; set; }
        public WSStatusController(ApplicationDBContext context)
        {
            _context = context;
            
        }
        [HttpGet("status")]
        public async Task<IActionResult> GetStatus(int id)
        {
            // Simulate heavy database work
            //await Task.Delay(2000);
            
            this.WebsiteInfo = _context.WebSite.Include(t => t.WebSiteUpTimes).FirstOrDefault(w => w.ID == id)!;
            string strUpTime=string.Format("{0:n2}%", (this.WebsiteInfo.WebSiteUpTimes.OrderByDescending(o => o.ResultDate).Take(30).Where(w => w.ResultGroup == "فعال").Count() + (30 - (this.WebsiteInfo.WebSiteUpTimes.OrderByDescending(o => o.ResultDate).Count() > 30 ? 30 : this.WebsiteInfo.WebSiteUpTimes.OrderByDescending(o => o.ResultDate).Count()))) / (/*item.WebSiteUpTimes.Count*1.0*/30.0) * 100);


            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (HttpClient client = new HttpClient(handler))
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                try
                {
                    HttpResponseMessage response = client.GetAsync(WebsiteInfo.URL).Result;
                    //var imageBytes = response.Content.ReadAsByteArrayAsync();

                    //var fileName = $"image_{Guid.NewGuid()}.webp";
                    //var filePath = Path.Combine("wwwroot/uploads", fileName);
                    //await System.IO.File.WriteAllBytesAsync( filePath,imageBytes.Result);

                    if (response.IsSuccessStatusCode)
                    {
                        this.CurrentState = "<p class='alert alert-success'>" + "وضعیت فعلی: " + "فعال" + "</p>";
                    }
                    else
                    {

                        using (Ping myPing = new Ping())
                        {
                            PingReply reply = myPing.Send(WebsiteInfo.URL.Substring(7), 10000);
                            if (reply != null)
                            {
                                if (reply.Status == IPStatus.Success)
                                    this.CurrentState = "<p class='alert alert-warning'>" + "وضعیت فعلی: " + " فعال  - Ping " + "<br/>" + reply.Address + "</p>";
                                else
                                    this.CurrentState = "<p class='alert alert-danger'>" + "وضعیت فعلی: " + "غیر فعال" + " Ping  " + reply.Status + "</p>";

                            }
                            else
                            {
                                this.CurrentState = "<p class='alert alert-danger'>" + "وضعیت فعلی: " + "غیر فعال" + "</p>";
                            }
                        }
                    }
                }
                catch
                {
                    this.CurrentState = "<p class='alert alert-danger'>" + "وضعیت فعلی: " + "غیر فعال" + "</p>";
                }
            }

            return Ok(new { lastStatus = this.CurrentState });
        }
    }
}
