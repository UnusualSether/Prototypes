using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using System.Linq;

[Serializable]
public class Room
{

   
    enum DoorTilePos
    {
        Backmost,
        Leftmost,
        RightMost,
        Forthmost
    }

    public GameObject roomPrefab;

    // Array to enum works clockwise starting from the back : Backmost -> Leftmost ->Forthmost -> Rightmost
    public Transform[] doorTiles = new Transform[4];

   

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

        GetDoorTiles();

        void GetDoorTiles()
        {

            List<Transform> unsortedTiles = new List<Transform>();
            //For rooms with DoorTiles set in the inspector
            foreach(Transform tile in roomPrefab.transform)
            {
                if (tile.CompareTag("DoorTile"))
                {
                   unsortedTiles.Add(tile);
                }
            }

            if (unsortedTiles.Count == 0)
            {
                return;
            }

            doorTiles[(int)DoorTilePos.Backmost] = unsortedTiles.OrderByDescending(e => e.transform.position.y).FirstOrDefault();

            doorTiles[(int)DoorTilePos.Leftmost] = unsortedTiles.OrderByDescending (e => e.transform.position.x).LastOrDefault();

            doorTiles[(int)DoorTilePos.RightMost] = unsortedTiles.OrderByDescending(e => e.transform.position.x).FirstOrDefault();

            doorTiles[(int)DoorTilePos.Forthmost] = unsortedTiles.OrderByDescending(e => e.transform.position.y).LastOrDefault();


        }
        

    }

}
