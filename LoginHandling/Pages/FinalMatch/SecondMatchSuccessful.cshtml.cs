using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoginHandling.Pages.FinalMatch
{
    public class SecondMatchSuccessfulModel : PageModel
    {
        public void OnGet()
        {
            // The row has to be deleted from the db
            // await DeleteCode();
        }
    }
}
