using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ThreeDGameHandler : MonoBehaviour
{

    public GameObject roomPrefab;
    public List<GameObject> spawnedRooms = new List<GameObject>();

    public Queue<GameObject> roomsQueue = new Queue<GameObject>();


    [ContextMenu("SpawnNewRoom")]
    public void CreateNewRoom()
    {
         var roomToSpawnNextTo = spawnedRooms.Last();

        Vector3 spawnpoint = new Vector3(roomToSpawnNextTo.transform.position.x, roomToSpawnNextTo.transform.position.y, roomToSpawnNextTo.transform.position.z + 5);

        var newlyCreatedRoom = Instantiate(roomPrefab, spawnpoint, new Quaternion(roomToSpawnNextTo.transform.rotation.x,roomToSpawnNextTo.transform.rotation.y,roomToSpawnNextTo.transform.rotation.z,roomToSpawnNextTo.transform.rotation.w));

        spawnedRooms.Add(newlyCreatedRoom);
    }

}
