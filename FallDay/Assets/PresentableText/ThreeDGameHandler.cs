using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;


public partial class ThreeDGameHandler : MonoBehaviour
{
    [Header("ROOM TRACKING")]
    [Space(10)]
    public GameObject roomPrefab;
    public List<GameObject> spawnedRooms = new List<GameObject>();

    public Queue<GameObject> roomsQueue = new Queue<GameObject>();

    public GameHandler handler;


    /// <summary>
    /// This is a function which you can call on any other partial ThreeDGameHandler script, it is basically a 
    ///start which doesn't cause conflict between partial classes. Basically, use this instead of Start().
    /// </summary>
    partial void OnStartExtendRoomCulling();

    partial void OnStartExtendRailMethod();



    private void Start()
    {
        OnStartExtendRailMethod();

        OnStartExtendRoomCulling();
    }


    void OnEnable() 
    { 
        //Room Culling Events
        CharacterMove.PlayerMoved += CullOldRooms; CharacterMove.PlayerMoved += CreateNewRoom;
        //On Rails Events
        handler.PlayerKilledAllZombies += EncounterEnd;
        
    }

    void OnDisable() 
    {
        //Room Culling Events
        CharacterMove.PlayerMoved -= CullOldRooms; CharacterMove.PlayerMoved -= CreateNewRoom;
        //On Rails Events
        handler.PlayerKilledAllZombies -= EncounterEnd;

    }

    [ContextMenu("SpawnNewRoom")]
    public void CreateNewRoom()
    {
        var roomToSpawnNextTo = roomsQueue.Last();

        Vector3 spawnpoint = new Vector3(roomToSpawnNextTo.transform.position.x, roomToSpawnNextTo.transform.position.y, roomToSpawnNextTo.transform.position.z + 5);

        var newlyCreatedRoom = Instantiate(roomPrefab, spawnpoint, new Quaternion(roomToSpawnNextTo.transform.rotation.x,roomToSpawnNextTo.transform.rotation.y,roomToSpawnNextTo.transform.rotation.z,roomToSpawnNextTo.transform.rotation.w));

        spawnedRooms.Add(newlyCreatedRoom);

        roomsQueue.Enqueue(newlyCreatedRoom);
    }


   

}
