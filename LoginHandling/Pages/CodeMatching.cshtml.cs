using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoginHandling.Pages
{
    [Authorize]
    public class CodeMatchingModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
