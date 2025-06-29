using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (Username == "admin" && Password == "admin")
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToPage("/Index");
            }
            ModelState.Clear();
            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return Page();
        }
    }
}
