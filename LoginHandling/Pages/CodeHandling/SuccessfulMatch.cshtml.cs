using LoginHandling.Models;
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
    private OpenDoorRequest _openDoorRequest;
    public string CloudGeneratedCode { get; set; }
    public string CodeInsertedOnDoorByUser { get; set; }

    static HttpClient _client;
    private Timer _timer;
    private CancellationTokenSource _cancellationTokenSource;

    // Fields useful for cloud to device messages
    //static ServiceClient serviceClient;
    //static string connectionString = "HostName=Pi-Cloud.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=sx1De7uIm+lA/4E1olGyS1tvJjpKt/vzlDbfOs5eqHY=";
    //public Packet Packet { get; private set; }
    //static string targetDevice;


    public void OnGet(string jsonOpenDoorRequest)
    {
        if (jsonOpenDoorRequest is null)
        {
            throw new ArgumentNullException(nameof(jsonOpenDoorRequest));
        }
        _openDoorRequest = JsonSerializer.Deserialize<OpenDoorRequest>(jsonOpenDoorRequest);

        Console.WriteLine("CloudGeneratedCode that should be printed" + _openDoorRequest.CloudGeneratedCode);

        // To expose the code on the UI
        CloudGeneratedCode = _openDoorRequest.CloudGeneratedCode;

        // Http client initialization
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://localhost:7295/");
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _cancellationTokenSource = new CancellationTokenSource();
        _timer = new Timer(DoApiCall, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));


        // Get the device information
        // Here a query has to be done to retrieve the Gateway.DeviceId that matches this Gateway.Id
        //targetDevice = _openDoorRequest.DeviceId;
        //Console.WriteLine("Target device id: " + targetDevice);

        //Console.WriteLine("Send Cloud-to-Device message\n");
        //serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

        //SendCloudToDeviceMessageAsync(_openDoorRequest.CloudGeneratedCode.ToString()).Wait();

        // Dopo averlo mandato, dovrò aspettare qui in await per la risposta da parte del PIC 
        // (altrimenti ogni volte che lo user clicca "refresh page" il messaggio viene inviato
        // di nuovo al device


    }

    //private async static Task SendCloudToDeviceMessageAsync(string code)
    //{
    //    var commandMessage = new Message(Encoding.ASCII.GetBytes("Random ack code: " + code));
    //    await serviceClient.SendAsync(targetDevice, commandMessage);
    //}


    private async void DoApiCall(object state)
    {
        // Now we have to wait for the user to insert the code on the PIC keyboard

        // Make the API call
        string path = $"api/DoorOpenRequest/{_openDoorRequest.Id}";

        var response = await GetOpenDoorRequestAsync(path);

        if (response == null)
        {
            Console.WriteLine("Error: the call was not successful");
        }
        else
        {
            Console.WriteLine("Code for now: " + response.CodeInsertedOnDoorByUser);
            if(response.CodeInsertedOnDoorByUser != null)
            {
                CodeInsertedOnDoorByUser = response.CodeInsertedOnDoorByUser;
                // Stop requesting data
                StopApiCalls();
            }
        }
        
    }

    private void StopApiCalls()
    {
        _timer?.Dispose();
        _cancellationTokenSource.Cancel();
        // Perform any additional cleanup or logic if needed
    }

    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        // Stop the timer and cancel the API calls when the request is aborted
        HttpContext.RequestAborted.Register(() =>
        {
            StopApiCalls();
        });

        base.OnPageHandlerExecuting(context);
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

}