using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SatraWebApplication.Model;
using System.ComponentModel.DataAnnotations;

namespace SatraWebApplication.Pages
{
    public class Step2Model : PageModel
    {
        private readonly FormState _formState;

        public Step2Model(FormState formState)
        {
            _formState = formState;
        }

        [BindProperty]
        [Required]
        public string Step2Data { get; set; }

        public string Step1Data => _formState.Step1Data;

        [BindProperty(SupportsGet = true)]
        public bool ShowResult { get; set; }

        public void OnGet()
        {
            Step2Data = _formState.Step2Data;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _formState.Step2Data = Step2Data;
            return RedirectToPage(new { ShowResult = true });
        }
    }
}
