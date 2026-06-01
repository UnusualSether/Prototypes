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
        room.GetComponent<RoomDissipate>().StartRoomDestroyCoroutine();


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

        farthestRoom.GetComponent<RoomDissipate>().StartRoomDestroyCoroutine();



        if (spawnedRooms.Count < 0)
        {
            spawnedRooms.Clear();
        }



    }

    public void CullRoomList()
    {
        toDestroyThreeWay = previousThreeWay;

        for (int i = 0; i < toDestroyThreeWay.Count(); i++)
        {
            Destroy(toDestroyThreeWay[i]);
        }

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
                waypoint.belongingRoom.GetComponent<RoomDissipate>().StartRoomDestroyCoroutine();
            }
        }

        

    }

    
    
}
