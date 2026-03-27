using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System;

public partial class ThreeDGameHandler
{


   
    public int maxNumberOfRooms;

    public static event Action RoomSetupComplete;



    public void DestroyRoom(GameObject room)
    {
        Destroy(room);

        spawnedRooms.Remove(room);

    }
    partial void OnStartExtendRoomCulling()
    {
        roomsQueue.Enqueue(spawnedRooms.First());

        RoomSetup();

    }

    void OnEnable() { CharacterMove.PlayerMoved += CullOldRooms; CharacterMove.PlayerMoved += CreateNewRoom; }

    void OnDisable() { CharacterMove.PlayerMoved -= CullOldRooms; CharacterMove.PlayerMoved -= CreateNewRoom; }




    private void RoomSetup()
    {


        while (roomsQueue.Count != maxNumberOfRooms)
        {
            CreateNewRoom();
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
}
