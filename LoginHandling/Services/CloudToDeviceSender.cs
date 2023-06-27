using Microsoft.Azure.Devices;

namespace LoginHandling.Services;

public class CloudToDeviceSender
{
    // IoT Hub connection string
    private readonly string _connectionString;
    // Device Id
    public string deviceId = "<your device Id>";

    public CloudToDeviceSender(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("azuredb");
    }

    public async void GetDeviceConnString()
    {

    }
}
