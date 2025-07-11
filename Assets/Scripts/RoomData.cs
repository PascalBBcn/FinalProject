using UnityEngine;
// Class used to hold information about each room, including RoomType
public class RoomData
{
    public RoomType roomType;
    public RectInt bounds;
    public bool isOrganic;

    // Constructor
    public RoomData(RectInt bounds, RoomType roomType = RoomType.Normal, bool isOrganic = false)
    {
        this.roomType = roomType;
        this.bounds = bounds;
        this.isOrganic = isOrganic;
    }
}
