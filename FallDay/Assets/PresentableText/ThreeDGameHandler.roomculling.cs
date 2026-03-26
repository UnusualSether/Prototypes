using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

public partial class ThreeDGameHandler
{
    public int maxNumberOfRooms;




    public void DestroyRoom(GameObject room)
    {
        Destroy(room);


    }
    private void Start()
    {
        roomsQueue.Enqueue(spawnedRooms.First());

        RoomSetup();

    }

    void OnEnable() { CharacterMove.PlayerMoved += CullOldRooms; CharacterMove.PlayerMoved += CreateNewRoom; }

    void OnDisable() { CharacterMove.PlayerMoved -= CullOldRooms; CharacterMove.PlayerMoved -= CreateNewRoom; }




    private void RoomSetup()
    {


        while (roomsQueue.Count != 3)
        {
            CreateNewRoom();
        }
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
