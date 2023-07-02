using LoginHandling.Models;
using LoginHandling.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Net.Http.Headers;

namespace LoginHandling.Pages;

[Authorize(Roles = "Admin")]
public class AccessesViewModel : PageModel
{
    private readonly ApiProxyService _apiProxyService;
    public List<AccessExtended> Accesses { get; set; }

    public AccessesViewModel(IHttpClientFactory httpClientFactory, ApiProxyService apiProxyService)
    {
        _apiProxyService = apiProxyService;
    }

    public async Task OnGet()
    {
        // Try to retrieve data
        var accesses = await _apiProxyService.GetAccessesAsync();
        if (accesses != null)
        {
            Accesses = accesses.ToList();
        }
        
    }
}
