using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SatraWebApplication.Data;
using SatraWebApplication.Model;
using System.Net.NetworkInformation;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Diagnostics.Eventing.Reader;

namespace SatraWebApplication.Pages
{
    public class WebsiteDetailModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public readonly IWebHostEnvironment _env;
        public WebSite WebsiteInfo { get; set; }
        public string LastScreenShotImage { get; set; }
        public bool HasScreenShot { get; set; } = false;
        public bool ScreenShotExist(int id)
        {
            string filePath = Path.Combine(_env.WebRootPath, "ScreenShot", id.ToString() + ".png");
            if (System.IO.File.Exists(filePath))
            {
                return true;
            }
            return false;
        }
        public string CurrentState { get; set; }
        public WebsiteDetailModel(ApplicationDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public  async Task OnGetAsync(int ID)
        {
            this.WebsiteInfo = _context.WebSite.Include(t => t.WebSiteUpTimes).FirstOrDefault(w => w.ID == ID)!;
            this.LastScreenShotImage = this.WebsiteInfo.WebSiteUpTimes.OrderByDescending(t => t.ID).FirstOrDefault()!.ID.ToString();
            string filePath = Path.Combine(_env.WebRootPath, "ScreenShot", LastScreenShotImage+".png");
            if (System.IO.File.Exists(filePath))
            {
                HasScreenShot = true;
            }
            
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            
            using (HttpClient client =new HttpClient(handler))
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
                                if(reply.Status==IPStatus.Success)
                                    this.CurrentState = "<p class='alert alert-warning'>" + "وضعیت فعلی: " + " فعال  - Ping " + "<br/>" +reply.Address+ "</p>";
                                else
                                    this.CurrentState = "<p class='alert alert-danger'>" + "وضعیت فعلی: " + "غیر فعال" +" Ping  "+reply.Status+ "</p>";

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

        }
    }
}
