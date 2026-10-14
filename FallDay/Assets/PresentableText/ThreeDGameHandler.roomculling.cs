using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

public partial class ThreeDGameHandler
{
    public int maxNumberOfRooms;

    public static event Action RoomSetupComplete;
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

    GameObject previousPlayerChosenRoom;
    GameObject rootRoomNext;

    public void PlayerMovingTowardsNewRoom(Waypoint waypointPlayerHasArrivedAt)
    {

        var rootRoom = waypointPlayerHasArrivedAt.belongingRoom;
        rootRoomNext = rootRoom;

        CreateThreeWay(rootRoom);
    }
    
}
