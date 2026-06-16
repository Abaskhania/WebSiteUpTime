using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace SatraWebApplication.Pages
{
    public class testModel : PageModel
    {
        [BindProperty,Required]
        public string? State { get; set; }
        public string[] States = new[] { "فعال", "غیر فعال" };

        public void OnGet()
        {
            using (Ping pinger = new Ping())
            {
                PingReply reply = pinger.Send("netfiliim.ir");
                //reply.Status == IPStatus.Success;
            }
        }
        public void OnPost()
        {
            
        }
    }
}
