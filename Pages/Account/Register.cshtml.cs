using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SatraWebApplication.Data;
using SatraWebApplication.Model;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace SatraWebApplication.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class RegisterModel : PageModel
    {

        private readonly ApplicationDBContext _context;

        public RegisterModel(ApplicationDBContext context)
        {
            _context = context;
        }
        [BindProperty]
        public RegisterInput Input { get; set; }

        public class RegisterInput
        {
            [Required(ErrorMessage = "نام کاربری الزامی است")]
            [Display(Name = "نام کاربری")]
            public string Username { get; set; }

           
            [Required(ErrorMessage = "رمز عبور الزامی است")]
            [DataType(DataType.Password)]
            [Display(Name = "رمز عبور")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز عبور باید حداقل ۶ کاراکتر باشد")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "تکرار رمز عبور")]
            [Required(ErrorMessage = "تکرار رمز عبور الزامی است")]
            [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن مطابقت ندارند")]
            public string ConfirmPassword { get; set; }
        }
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string hashPassword = GetBCryptHash(this.Input.Password);
            SatraUser uExists = _context.SatraUser.FirstOrDefault(u => u.Username == this.Input.Username)!;
            if (uExists == null)
            {
                SatraUser u = new SatraUser { Username = this.Input.Username, Password = hashPassword, Role = "User" };
                _context.SatraUser.Add(u);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "ثبت‌ نام با موفقیت انجام شد!";
            }
            else
            {
                ModelState.AddModelError("کاربر تکراری", "نام کاربری فوق قبلا در سامانه ثبت شده است.");
                return Page();

            }
                // در اینجا منطق ذخیره در دیتابیس را بنویسید
                

            return RedirectToPage();
        }
        private string GetBCryptHash(string input)
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
