using System;
using UnityEngine;

[Serializable]
public class Room
{

   


    public GameObject roomPrefab;



   

    public Vector3 GetSize()
    {
        return roomPrefab.transform.lossyScale;
    }

    public Room(RoomData data)
    {
        if (roomPrefab == null)
        {
            roomPrefab = data.prefab;
        }
        

    }

}
