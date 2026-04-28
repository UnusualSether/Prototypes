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
            CreateThreeWay();
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

    
}
