using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoginHandling.Pages;

public class SuccessfulMatchModel : PageModel
{
    public string RandomAckCode { get; private set; }

    public void OnGet()
    {
        Random random = new Random();
        for (int i = 0; i < 5; i++)
        {
            RandomAckCode += random.Next(1, 10).ToString(); // Generate a random number between 1 and 9
        }
    }
}
