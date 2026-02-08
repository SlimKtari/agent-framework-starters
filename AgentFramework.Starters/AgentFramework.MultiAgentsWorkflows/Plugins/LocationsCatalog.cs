using System.ComponentModel;

namespace AgentFramework.MultiAgentsWorkflows.Plugins;

public record Room(string Name, int Capacity, bool IsAvailable, string LocationName);

public class LocationsCatalog
{
    [Description("Find available rooms at a given location within a time range. " +
        "Returns room details including capacity and availability status.")]
    public List<Room> LookupRooms(
        [Description("Name of the location or building where rooms should be searched (for example: Accord Arenas or Cegid Boulogne).")]
        string locationName,

        [Description("Start date and time of the requested reservation period.")]
        DateTime startDateTime,

        [Description("End date and time of the requested reservation period.")]
        DateTime endDateTime)
    {
        return
        [
            new Room("Contoso Conference Room A", 10, true, locationName),
            new Room("Contoso Conference Room B", 20, false, locationName),
            new Room("Contoso Conference Room C", 15, true, locationName)
        ];
    }
}