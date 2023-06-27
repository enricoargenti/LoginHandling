namespace LoginHandling.Models;

public class AccessExtended
{
    public string Username { get; set; }
    public int DoorId { get; set; }
    public string DeviceId { get; set; }
    public string BuildingName { get; set; }
    public string BuildingDescription { get; set; }
    public DateTime AccessRequestTime { get; set; }

    public AccessExtended()
    {

    }
}
