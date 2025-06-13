using System.Collections.Generic;
using UnityEngine;

// Class used to assign rooms to different roomTypes, which is then used for
// intelligent spawning
public class RoomAssigner
{
    public static List<RoomData> AssignRooms(List<RectInt> rooms, Vector2Int furthestRoom)
    {
        List<RoomData> roomData = new List<RoomData>();

        int bossRoomIndex = 1;
        for (int i = 1; i < rooms.Count; i++)
        {
            if (Vector2Int.FloorToInt(rooms[i].center) == furthestRoom) bossRoomIndex = i;
        }
        List<int> validChestRoomIndices = new List<int>();
        for (int i = 1; i < rooms.Count; i++)
        {
            if (i != bossRoomIndex) validChestRoomIndices.Add(i);
        }
        int chestRoomIndex = validChestRoomIndices[Random.Range(0, validChestRoomIndices.Count - 1)];

        // Room assignment via tagging by roomType
        for (int i = 0; i < rooms.Count; i++)
        {
            RoomType roomType = RoomType.Normal;
            if (i == 0) roomType = RoomType.Start;
            else if (i == chestRoomIndex) roomType = RoomType.Chest;
            else if (i == bossRoomIndex) roomType = RoomType.Boss;

            roomData.Add(new RoomData(rooms[i], roomType));
        }

        return roomData;
    }
}
