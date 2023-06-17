using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using LoginHandling.Models;
using System.Net.Http.Headers;
using System;
using Microsoft.AspNetCore.Identity;
using DbAccessApplication.Models;

namespace LoginHandling.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    private readonly UserManager<IdentityUser> _userManager;

    private string _currentUserId { get; set; }
    private string _userInsertedCode { get; set; }
    public string DeviceSentCode { get; set; }

    private OpenDoorRequest _openDoorRequest { get; set; }
    private UserPermissions _userPermissions { get; set; }
    private bool _hasPermissions { get; set; }

    // Client initialization
    static HttpClient _client;

    public IndexModel(ILogger<IndexModel> logger, UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;

        // Set the Http client
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://localhost:7295/");
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<IActionResult> OnPostCodeMatcher()
    {
        // Get the current user id
        IdentityUser currentUser = await _userManager.GetUserAsync(User);
        _currentUserId = currentUser.Id;
        Console.WriteLine("_currentUserId at the beginning: " + _currentUserId);

        // Access the form input value
        _userInsertedCode = Request.Form["userInsertedCode"];

        // Once we have the code inserited by the user,
        // we have to search for the corresponding code inside the database

        await GetCode();

        Console.WriteLine($"Convertito a stringa: {DeviceSentCode}");
        Console.WriteLine($"Codice inserito dall'utente: {_userInsertedCode}");
        bool vediamo = _userInsertedCode.Equals(DeviceSentCode);
        Console.WriteLine("Valore boolean odel confronto: " + vediamo);


        if (_userInsertedCode.Equals(DeviceSentCode))  // ADD ALSO THE CONTROL ON THE DATETIME!
        {
            // The first control is on the permission given to the current user to access the building

            // APIs call to get the user permissions
            await GetUserPermissions();

            if(_hasPermissions)
            {
                Console.WriteLine($"The user {_userPermissions.UserName} has permissions to get inside");
            }
            else
            {
                Console.WriteLine($"The user has no permission to get inside");
                // Redirect to another page
                return RedirectToPage("/CodeHandling/NoPermission");
            }

            // Control that the token is still valid
            TimeSpan difference = DateTime.Now.Subtract(_openDoorRequest.AccessRequestTime);
            TimeSpan threeMinutes = TimeSpan.FromMinutes(3);
            Console.WriteLine("Difference: " + difference);
            Console.WriteLine("Three minutes: " + threeMinutes);

            if (difference > threeMinutes)
            {
                Console.WriteLine("Error: the code is not valid anymore");

                // APIs call to delete the expired row
                await DeleteCode();

                // Redirect to another page
                return RedirectToPage("/CodeHandling/CodeExpired");
            }
            else
            {
                // Redirect to another page on successful match
                string jsonOpenDoorRequest = JsonSerializer.Serialize(_openDoorRequest);
                return RedirectToPage("/CodeHandling/SuccessfulMatch", new { jsonOpenDoorRequest = jsonOpenDoorRequest });
            }

        }
        else
        {
            // Redirect to another page on failed match
            return RedirectToPage("/CodeHandling/FailedMatch");
        }
    }

    public async Task GetCode()
    {
        try
        {

            string path = $"api/DoorOpenRequest/deviceGeneratedCode/{_userInsertedCode}";

            _openDoorRequest = await GetOpenDoorRequestAsync(path);

            if (_openDoorRequest != null)
            {
                Console.WriteLine($"OpenDoorRequest found: {_openDoorRequest.Id}");
                DeviceSentCode = _openDoorRequest.DeviceGeneratedCode;
            }
            else
            {
                Console.WriteLine("OpenDoorRequest not found.");
                DeviceSentCode = null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

public async Task DeleteCode()
{
    try
    {
        string path = $"api/DoorOpenRequest/{_openDoorRequest.Id}";
        await DeleteOpenDoorRequestAsync(path);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}

public async Task GetUserPermissions()
{
    try
    {
        string path = $"api/DoorOpenRequest/user/{_currentUserId}/deviceId/{_openDoorRequest.DeviceId}";
        Console.WriteLine("path to get permissions: " + path);

        _userPermissions = await GetUserPermissionsAsync(path);
        Console.WriteLine("_userPermissions: " + _userPermissions);

        if (_userPermissions != null)
        {
            _hasPermissions = true;
        }
        else
        {
            _hasPermissions = false;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}


    static async Task<OpenDoorRequest> GetOpenDoorRequestAsync(string path)
    {
        OpenDoorRequest openDoorRequest = null;
        HttpResponseMessage response = await _client.GetAsync(path);
        if (response.IsSuccessStatusCode)
        {
            openDoorRequest = await response.Content.ReadAsAsync<OpenDoorRequest>();
        }
        return openDoorRequest;
    }

    static async Task DeleteOpenDoorRequestAsync(string path)
    {
        HttpResponseMessage response = await _client.DeleteAsync(path);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Expired row successfully deleted");
        }
        else
        {
            Console.WriteLine("Error: the delete action could not be processed for any reason");
        }
    }

    static async Task<UserPermissions> GetUserPermissionsAsync(string path)
    {
        UserPermissions userPermissions = null;
        HttpResponseMessage response = await _client.GetAsync(path);
        if (response.IsSuccessStatusCode)
        {
            userPermissions = await response.Content.ReadAsAsync<UserPermissions>();
        }
        return userPermissions;
    }

}