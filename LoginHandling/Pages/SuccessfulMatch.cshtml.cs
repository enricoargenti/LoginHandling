using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Devices;
using System.Text;
using LoginHandling.Services;

namespace LoginHandling.Pages;

public class SuccessfulMatchModel : PageModel
{
    public string RandomAckCode { get; private set; }
    
    // Fields useful for cloud to device messages
    static ServiceClient serviceClient;
    static string connectionString = "HostName=Pi-Cloud.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=sx1De7uIm+lA/4E1olGyS1tvJjpKt/vzlDbfOs5eqHY=";
    static string targetDevice = "Device1";

    // Fields useful to check if the code is on the queue on the IoT Hub
    QueueListener queueListener;
    public string ReceivedCode { get; private set; }

    public void OnGet()
    {
        // Check if the code is on the queue
        queueListener = new QueueListener();
        queueListener.ListenQueue().Wait();
        ReceivedCode = queueListener.ReceivedMessage;

        // Random code generation
        Random random = new Random();
        for (int i = 0; i < 5; i++)
        {
            RandomAckCode += random.Next(1, 10).ToString(); // Generate a random number between 1 and 9
        }

        // Random code sending to the device
        Console.WriteLine("Send Cloud-to-Device message\n");
        serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

        //Console.WriteLine("Press any key to send a C2D message.");
        //Console.ReadLine();
        SendCloudToDeviceMessageAsync(RandomAckCode).Wait();
        //Console.ReadLine();
    }

    private async static Task SendCloudToDeviceMessageAsync(string code)
    {
        var commandMessage = new
         Message(Encoding.ASCII.GetBytes("Random ack code: " + code));
        await serviceClient.SendAsync(targetDevice, commandMessage);
    }
}
