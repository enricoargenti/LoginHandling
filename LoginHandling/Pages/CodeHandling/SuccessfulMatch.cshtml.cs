using LoginHandling.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Devices;
using System.Text;
using System.Text.Json;

namespace LoginHandling.Pages.CodeHandling;

[Authorize]
public class SuccessfulMatchModel : PageModel
{
    private OpenDoorRequest _openDoorRequest;
    public int CloudGeneratedCode { get; set; }

    // Fields useful for cloud to device messages
    static ServiceClient serviceClient;
    static string connectionString = "HostName=Pi-Cloud.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=sx1De7uIm+lA/4E1olGyS1tvJjpKt/vzlDbfOs5eqHY=";
    public Packet Packet { get; private set; }
    static string targetDevice;

    public void OnGet(string jsonOpenDoorRequest)
    {
        if (jsonOpenDoorRequest is null)
        {
            throw new ArgumentNullException(nameof(jsonOpenDoorRequest));
        }
        _openDoorRequest = JsonSerializer.Deserialize<OpenDoorRequest>(jsonOpenDoorRequest);

        // To expose the code on the UI
        CloudGeneratedCode = _openDoorRequest.CloudGeneratedCode;

        // Get the device information
        targetDevice = _openDoorRequest.GatewayId.ToString();

        Console.WriteLine("Send Cloud-to-Device message\n");
        serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

        //SendCloudToDeviceMessageAsync(_openDoorRequest.CloudGeneratedCode.ToString()).Wait();
    }

    private async static Task SendCloudToDeviceMessageAsync(string code)
    {
        var commandMessage = new Message(Encoding.ASCII.GetBytes("Random ack code: " + code));
        await serviceClient.SendAsync(targetDevice, commandMessage);
    }
}
