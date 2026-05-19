using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

public partial class ThreeDGameHandler
{


   
    public int maxNumberOfRooms;

    public static event Action RoomSetupComplete;



    public void DestroyRoom(GameObject room)
    {
        Destroy(room);

        

    }
    partial void OnStartExtendRoomCulling()
    {
        roomsQueue.Enqueue(spawnedRooms.First().roomPrefab);

        RoomSetup();

    }



    private void RoomSetup()
    {


        if (!playerChooseSystem)
        {
            while (roomsQueue.Count != maxNumberOfRooms)
            {
                CreateNewRoom();
            }
        }

        else
        {
            CreateThreeWay(roomsQueue.Dequeue());
        }

            RoomSetupComplete?.Invoke();
    }
    public bool RoomsNeedCulling()
    {

        if (roomsQueue.Count > maxNumberOfRooms)
        {
            return true;
        }


        return false;
    }

    public void CullOldRooms()
    {
        if (!RoomsNeedCulling())
        {
            return;
        }

        var farthestRoom = roomsQueue.Dequeue();

        DestroyRoom(farthestRoom);



        if (spawnedRooms.Count < 0)
        {
            spawnedRooms.Clear();
        }



    }

    public void CullRoomList()
    {
        var roomsToCull = spawnedRooms.Take(3);

        spawnedRooms = spawnedRooms.Except(roomsToCull).ToList();
    }

    GameObject playerChosenRoom;

    public void PlayerArrivedToNewRoom(Waypoint waypointPlayerHasArrivedAt)
    {

        var rootRoom = waypointPlayerHasArrivedAt.belongingRoom;

        playerChosenRoom = rootRoom;
        

        CreateThreeWay(rootRoom);

        


    }
    public void DestroyUnusedRooms(List<Waypoint> waypoints)
    {
        
        foreach (var waypoint in waypoints)
        {
            if (waypoint.belongingRoom != playerChosenRoom)
            {
                Destroy(waypoint.belongingRoom);
            }
        }

    }

    
    
}
