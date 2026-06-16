using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SatraWebApplication.Model;
using System.ComponentModel.DataAnnotations;

namespace SatraWebApplication.Pages
{
    public class Step1Model : PageModel
    {
        private readonly FormState _formState;

        public Step1Model(FormState formState)
        {
            _formState = formState;
        }

        [BindProperty]
        [Required]
        public string Step1Data { get; set; }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _formState.Step1Data = Step1Data;
            return RedirectToPage("Step2");
        }
    }
}
