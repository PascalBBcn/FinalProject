using UnityEngine;
// Class used to hold information about each room, including RoomType
public class RoomData
{
    public RoomType roomType;
    public RectInt bounds;

    // Constructor
    public RoomData(RectInt bounds, RoomType roomType = RoomType.Normal)
    {
        this.roomType = roomType;
        this.bounds = bounds;
    }
}
