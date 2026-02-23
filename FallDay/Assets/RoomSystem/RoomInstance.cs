using UnityEngine;

public class RoomInstance
{
    public RoomData data { get; }

    public bool roomCleared;

    public RoomInstance (RoomData data)
    {
        this.data = data;

        roomCleared = false;
    }
}
