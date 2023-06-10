using Azure.Messaging.ServiceBus;
using LoginHandling.Models;
using System.Threading.Tasks;

namespace LoginHandling.Services;

public class QueueListener
{
    private readonly string _queueName = "incomingrequestsopendoor";

    private readonly string __serviceBusConnString = "Endpoint=sb://gruppo4-pi-cloud.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=eTCoUTZbpTMk7HpOvBIbPO0gBns3yelg4+ASbFFMD8M=";

    // the client that owns the connection and can be used to create senders and receivers
    ServiceBusClient client;

    ServiceBusReceiver receiver;

    public string ReceivedMessage;

    public QueueListener()
    {
        client = new ServiceBusClient(__serviceBusConnString);
        receiver = client.CreateReceiver(_queueName);
    }

    public async Task ListenQueue()
    {
        try
        {
            // the received message is a different type as it contains some service set properties
            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();

            // Check the device-specific property
            //DeviceId = receivedMessage.ApplicationProperties["DeviceId"].ToString();
            //Console.WriteLine(DeviceId);

            // get the message body as a json string
            var messageJSON = receivedMessage.Body;

            ReceivedMessage = messageJSON.ToString();

            // Complete the message to remove it from the queue
            await receiver.CompleteMessageAsync(receivedMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            ReceivedMessage = "no-message";
        }
        finally
        {

        }

    }

}
