using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomInternalRefrences : MonoBehaviour
{
    // This class is meant to be a container for all the internal references of a room, such as the doors, the spawn points, the enemies, etc. It is meant to be used as a component of the room prefab,
    // and it will be used to access the internal references of the room from other scripts.
    public GameObject[] RoomWayPoints; //Refrence To WayPoints Avoids Using FindGameObjectsWithTag Multiple Times
    public DoorHandler DoorHandler; // The DoorHandler component of the room, which is used to handle the doors of the room. { Why Not Just Leve this single information in the Object Door? } _RLH107

    private int[] RoomPosition = new int[2]; // The position of the room in the grid, Refrenced in RoomHandler as follows: 0,2 | 1,2 | 2,2
                                             //                                                                            0,1 | 1,1 | 2,1
                                             //                                                                            0,0 | 1,0 | 2,0

    public bool DebugLogIsOn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the internal references of the room, such as the doors, the spawn points, the enemies, etc. If the references are not set in the inspector,
        if ( RoomWayPoints == null || RoomWayPoints.Length == 0)
        {
            GetRoomWayPoints();
            if (DebugLogIsOn)
            {
                Debug.Log($"[RoomInternalRefrences] RoomWayPoints length: {RoomWayPoints.Length}");
                foreach (var item in RoomWayPoints)
                {
                    Debug.Log($"[RoomInternalRefrences] RoomWayPoint: {item.name}");
                }
            }
        }
        if (DoorHandler == null)
        {
            DoorHandler = GetComponentInChildren<DoorHandler>();
            if (DebugLogIsOn)
            {
                Debug.Log($"[RoomInternalRefrences] DoorHandler: {DoorHandler.name}");
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void GetRoomWayPoints() // Saperated For Better Readability and Debugging possible To expand in additional functionality in the future (Additional object to get)
    {
        RoomWayPoints = GameObject.FindGameObjectsWithTag("Waypoint").ToArray();
    }

}

