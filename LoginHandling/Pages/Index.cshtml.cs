using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using LoginHandling.Models;
using System.Net.Http.Headers;
using System;

namespace LoginHandling.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private string _userInsertedCode { get; set; }
    public string deviceSentCode { get; set; }

    private OpenDoorRequest _openDoorRequest { get; set; }

    // Client initialization
    static HttpClient _client;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        _client = new HttpClient();
    }

    public async Task<IActionResult> OnPostCodeMatcher()
    {
        // Access the form input value
        _userInsertedCode = Request.Form["userInsertedCode"];

        // Once we have the code inserited by the user,
        // we have to search for the corresponding code inside the database

        await GetCode();

        // Ma qui devo mettere un while (_userInsertedCode == _deviceSentCode) ciclando su tutti i messaggi della coda.
        // Se alla fine nessuna corrispondenza è stata trovata, mando in failedMatch

        Console.WriteLine($"Convertito a stringa: {deviceSentCode}");
        Console.WriteLine($"Codice inserito dall'utente: {_userInsertedCode}");
        bool vediamo = _userInsertedCode.Equals(deviceSentCode);
        Console.WriteLine("Valore boolean odel confronto: " + vediamo);


        if (_userInsertedCode.Equals(deviceSentCode))  // ADD ALSO THE CONTROL ON THE DATETIME!
        {
            // Control that the token is still valid
            TimeSpan difference = _openDoorRequest.AccessRequestTime.Subtract(DateTime.Now);
            TimeSpan threeMinutes = TimeSpan.FromMinutes(3);
            Console.WriteLine("Difference: " + difference);
            Console.WriteLine("Three minutes: " + threeMinutes);

            if (difference > threeMinutes)
            {
                Console.WriteLine("Error: the code is not valid anymore");
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
        // Start the client that calls the APIs
        await RunAsync();

        try
        {

            string path = $"api/DoorOpenRequest/deviceGeneratedCode/{_userInsertedCode}";

            _openDoorRequest = await GetOpenDoorRequestAsync(path);

            if (_openDoorRequest != null)
            {
                Console.WriteLine($"OpenDoorRequest found: {_openDoorRequest.Id}");
                deviceSentCode = _openDoorRequest.DeviceGeneratedCode;
            }
            else
            {
                Console.WriteLine("OpenDoorRequest not found.");
                deviceSentCode = null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    // Http client initialization
    static async Task RunAsync()
    {
        _client.BaseAddress = new Uri("https://localhost:7295/");
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    // But here I should retrieve a single row where deviceGeneratedCode is the one that the user inserted
    // and if this call failes, it means that the user has to be redirected to the FailedMatch page
    // while if the call is successful the user is redirected to the SuccessfulPage and is inserted with a PUT call
    // in the table that handles the openDoorRequests
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


}