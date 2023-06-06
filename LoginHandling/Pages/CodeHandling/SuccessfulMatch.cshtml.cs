using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Devices;
using System.Text;

namespace LoginHandling.Pages.CodeHandling;

[Authorize]
public class SuccessfulMatchModel : PageModel
{
    public string RandomAckCode { get; private set; }
    public const string SessionKeyRandomAckCode = "_RandomAckCode";

    // Fields useful for cloud to device messages
    static ServiceClient serviceClient;
    static string connectionString = "HostName=Pi-Cloud.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=sx1De7uIm+lA/4E1olGyS1tvJjpKt/vzlDbfOs5eqHY=";
    static string targetDevice = "Device1";

    public void OnGet()
    {
        // Random code generation
        Random random = new Random();
        for (int i = 0; i < 5; i++)
        {
            // Generate a random number between 1 and 9
            RandomAckCode += random.Next(1, 10).ToString();
        }

        if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyRandomAckCode)))
        {
            // Store the code in the user's session (so that even if the page
            // is refreshed, the code will remain the same)
            HttpContext.Session.SetString(SessionKeyRandomAckCode, RandomAckCode);
        }
        RandomAckCode = HttpContext.Session.GetString(SessionKeyRandomAckCode);


        // Random code sending to the device
        Console.WriteLine("Send Cloud-to-Device message\n");
        serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

        SendCloudToDeviceMessageAsync(RandomAckCode).Wait();
    }

    private async static Task SendCloudToDeviceMessageAsync(string code)
    {
        var commandMessage = new Message(Encoding.ASCII.GetBytes("Random ack code: " + code));
        await serviceClient.SendAsync(targetDevice, commandMessage);
    }
}
