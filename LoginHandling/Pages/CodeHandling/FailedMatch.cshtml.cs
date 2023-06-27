using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoginHandling.Pages.CodeHandling;

[Authorize]
public class FailedMatchModel : PageModel
{
    public int AvailableTries { get; set; }
    public void OnGet(int availableTries)
    {
        AvailableTries = availableTries;
    }
}
