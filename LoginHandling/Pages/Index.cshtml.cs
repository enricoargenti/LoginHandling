using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoginHandling.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPostCodeMatcher()
        {
           // Here the program should call a function to search for the inserted code inside the IoT Hub

            // Redirect to another page on successful match
            return RedirectToPage("/SuccessfulMatch");
        }

    }
}