using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SatraWebApplication.Data;
using SatraWebApplication.Model;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SatraWebApplication.Pages.Account
{
    public class loginModel : PageModel
    {

        private readonly ApplicationDBContext _context;
        [BindProperty,Required(ErrorMessage ="نام کاربری را وارد کنید.")]
        public string Username { get; set; } = string.Empty;
        [BindProperty, Required(ErrorMessage = "رمز عبور را وارد کنید.")]
        public string Password { get; set; } = string.Empty;
        [BindProperty]
        public bool RememberMe { get; set; } = false;

        public loginModel(ApplicationDBContext context)
        {
           
            _context = context;
        
        }
        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(this.Password==null || this.Username==null)
            {
                //ModelState.AddModelError(string.Empty, "نام کاربری و رمز عبور را وارد کنید");
                return Page();
            }
            string hashPassword = GetBCryptHash(this.Password);
            SatraUser u = _context.SatraUser.FirstOrDefault(u => u.Username == this.Username)!;
            if (u is not null)
            {
                if (BCrypt.Net.BCrypt.Verify(this.Password, u.Password))
                {
                    var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name,Username),
                    new Claim(ClaimTypes.Role,u.Role)
                    };

                    var authProperty = new AuthenticationProperties();
                    if (RememberMe)
                    {
                        authProperty.IsPersistent = this.RememberMe;
                        authProperty.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14);
                    }
                    else
                    {
                        authProperty.IsPersistent = this.RememberMe;
                    }
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var log = new UserLoginLog
                    {
                        UserId = u.Username,
                        LoginTime = DateTime.UtcNow,
                        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                        IsSuccessful = true
                    };

                    _context.UserLoginLogs.Add(log);

                    await _context.SaveChangesAsync();
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity),authProperty);
                    
                    
                    return RedirectToPage("/Index");
                }
                else
                {
                    var logFailed = new UserLoginLog
                    {
                        UserId = u.Username,
                        LoginTime = DateTime.UtcNow,
                        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                        IsSuccessful = false
                    };

                    _context.UserLoginLogs.Add(logFailed);

                    await _context.SaveChangesAsync();
                    ModelState.AddModelError(string.Empty, "نام کاربری و رمز عبور نامعتبر است");
                    return Page();
                }

                //HttpContext.User?.Identity?.IsAuthenticated
            }
            var logAttemt = new UserLoginLog
            {
                UserId = this.Username,
                LoginTime = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                IsSuccessful = false
            };

            _context.UserLoginLogs.Add(logAttemt);

            await _context.SaveChangesAsync();
            ModelState.AddModelError(string.Empty, "نام کاربری و رمز عبور نامعتبر است");
            return Page();
        }

        private string GetBCryptHash(string  input)
        {
            using (var md5 = MD5.Create())
            {
                
                //byte[] inputByts = Encoding.UTF8.GetBytes(input);
                //byte[] hashBytes = md5.ComputeHash(inputByts);
                //return Convert.ToHexString(hashBytes);

            }
            return BCrypt.Net.BCrypt.HashPassword(input);
                    
        }
    }
}
