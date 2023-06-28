using LoginHandling.Models;
using LoginHandling.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Devices;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LoginHandling.Pages.CodeHandling;

[Authorize]
public class SuccessfulMatchModel : PageModel
{
    public string CloudGeneratedCode { get; set; }

    static HttpClient _client;
    private readonly ApiProxyService _apiProxyService;

    public SuccessfulMatchModel(ApiProxyService apiProxyService)
    {
        _apiProxyService = apiProxyService;
    }


    public async Task OnGet(string code)
    {
        OpenDoorRequest openDoorRequest = await _apiProxyService.GetDoorOpenRequestAsync(code);

        Console.WriteLine("CloudGeneratedCode that should be printed" + openDoorRequest.CloudGeneratedCode);

        // To expose the code on the UI
        CloudGeneratedCode = openDoorRequest.CloudGeneratedCode;
    }


    /*
    public async Task<IActionResult> CodesMatcher()
    {
        if (CodeInsertedOnDoorByUser == _openDoorRequest.CloudGeneratedCode)
        {
            // Try to set the log into the db
            await SetAccess();

            Console.WriteLine("/FinalMatch/SecondMatchSuccessful");
            // Redirect to another page
            return RedirectToPage("/FinalMatch/SecondMatchSuccessful");
        }
        else
        {
            Console.WriteLine("/FinalMatch/SecondMatchFailed");
            // Redirect to another page
            return RedirectToPage("/FinalMatch/SecondMatchFailed");
        }
    }
    */

    //Set Access (after all code match)
    /*
    public async Task SetAccess()
    {
        // Set the new access
        Access newAccess = new Access();
        newAccess.UserId = _openDoorRequest.UserId;
        newAccess.DoorId = _openDoorRequest.DoorId;
        newAccess.DeviceId = _openDoorRequest.DeviceId;
        newAccess.AccessRequestTime = _openDoorRequest.AccessRequestTime;

        string path = $"api/DoorOpenRequest/newaccess";

        HttpResponseMessage response = await _client.PostAsJsonAsync(path, newAccess);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("response: " + response.ToString());
        }
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("New access saved into logs table");
        }
    }
    */
}