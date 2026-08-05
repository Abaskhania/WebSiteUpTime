using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SatraWebApplication.Data;
using System.ComponentModel.DataAnnotations;
namespace SatraWebApplication.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        

        public LogoutModel(ApplicationDBContext context)
        {

            _context = context;

        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var login = await _context.UserLoginLogs
           .Where(x => x.UserId == User.Identity.Name)
           .OrderByDescending(x => x.LoginTime)
           .FirstOrDefaultAsync();

            if (login != null)
            {
                login.LogoutTime = DateTime.UtcNow;               

                await _context.SaveChangesAsync();
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Index");
        }
    }
}
