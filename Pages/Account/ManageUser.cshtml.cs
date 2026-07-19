using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SatraWebApplication.Data;
using SatraWebApplication.Model;

namespace SatraWebApplication.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class ManageUserModel : PageModel
    {
        private readonly ApplicationDBContext _context;

        public ManageUserModel(ApplicationDBContext context)
        {
            _context = context;
        }
        public IList<SatraUser> satraUsers { get; set; }
        public void OnGet()
        {
            this.satraUsers = _context.SatraUser.AsNoTracking().ToList();
        }
        public IActionResult OnGetChangeState(int id)
        {
            SatraUser u = _context.SatraUser.First(t => t.ID == id);
            u.IsValid = !u.IsValid;
            _context.SaveChanges();
            return RedirectToPage("ManageUser");
        }
    }
}
