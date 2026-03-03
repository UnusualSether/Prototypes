using NUnit.Framework;
using System.Diagnostics.Contracts;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;
using System.Linq;
using UnityEngine.Android;
using UnityEditor.SpeedTree.Importer;
using System;

public class RoomGenerationHandler : MonoBehaviour
{
    public GameObject chosenRoom;

    public List<RoomData> allRooms;

    public RoomSpawner spawner;

    public List<GameObject> spawnedRooms = new List<GameObject>();

    [ContextMenu ("Player Advancing")]
    public void SpawnNewRooms()
    {
        List<RoomData> options = get3Rooms();

        foreach ( var data in options)
        {
            Debug.Log($"{data.RoomName} will be one of the three rooms.");

            var newRoomInstance = new RoomInstance(data);

            spawner.SpawnRoom(newRoomInstance);

            


        }

    }


    [ContextMenu("Player Chose room")]
    public void DestroyRooms()
    {

        
    }

    List<RoomData> get3Rooms()
    {

        return allRooms.OrderBy(x => UnityEngine.Random.value).Take(3).ToList();
    }


}
