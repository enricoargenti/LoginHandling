using LoginHandling.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace LoginHandling.Pages;

public class AccessesViewModel : PageModel
{
    static HttpClient _client;
    public List<AccessExtended> Accesses { get; set; }

    public AccessesViewModel()
    {
        // Set the Http client
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://localhost:7295/");
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task OnGet()
    {
        // Try to retrieve data
        await GetCode();
        
    }

    public async Task GetCode()
    {
        try
        {
            string path = $"api/DoorOpenRequest/accesses";

            IEnumerable<AccessExtended> enumAccesses = await GetAccessesAsync(path);
            Accesses = enumAccesses.ToList();

            if (Accesses != null)
            {
                Console.WriteLine($"Accesses found: {Accesses.Count}");
            }
            else
            {
                Console.WriteLine("No access found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static async Task<IEnumerable<AccessExtended>> GetAccessesAsync(string path)
    {
        IEnumerable<AccessExtended> accesses = null;
        HttpResponseMessage response = await _client.GetAsync(path);
        if (response.IsSuccessStatusCode)
        {
            accesses = await response.Content.ReadAsAsync<IEnumerable<AccessExtended>>();
        }
        return accesses;
    }
}
