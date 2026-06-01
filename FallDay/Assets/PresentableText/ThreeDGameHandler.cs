using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity.VisualScripting;


public partial class ThreeDGameHandler : MonoBehaviour
{
    [Header("ROOM TRACKING")]
    [Space(10)]
    public GameObject roomPrefab;
    public List<Room> spawnedRooms = new List<Room>();

    public List<RoomData> possibleRooms = new List<RoomData>();

    public Queue<GameObject> roomsQueue = new Queue<GameObject>();

    public GameHandler handler;

    public Vector3 offsetToPlayer;

    GameObject[] current3Rooms = new GameObject[3];

    public bool playerChooseSystem;

    #region Current, Previous and new Three ways

    GameObject[] previousThreeWay = new GameObject[3];
    GameObject[] currentThreeWay = new GameObject[3];
    GameObject[] toDestroyThreeWay = new GameObject[3];

    #endregion




    /// <summary>
    /// This is a function which you can call on any other partial ThreeDGameHandler script, it is basically a 
    ///start which doesn't cause conflict between partial classes. Basically, use this instead of Start().
    /// </summary>
    partial void OnStartExtendRoomCulling();

    partial void OnStartExtendRailMethod();

    partial void OnStartExtendPlayerChoiceMethod();



    private void Start()
    {
        OnStartExtendRailMethod();

        OnStartExtendRoomCulling();

        OnStartExtendPlayerChoiceMethod();
    }


    void OnEnable() 
    { 
        //Room Culling Events
        CharacterMove.PlayerMoved += CullOldRooms; CharacterMove.PlayerMoved += CullRoomList; CharacterMove.PlayerMovingTowardsUnLoadedRoom += PlayerArrivedToNewRoom; CharacterMove.PlayerWillClearList += DestroyUnusedRooms;
        //On Rails Events
        handler.PlayerKilledAllZombies += EncounterEnd;
        //Player Events
        CharacterMove.PlayerHasReachedNextPoint += PlayerToEncounterGate; CharacterMove.PlayerHasReachedNextPoint += EndRails;
        //Player Choice Events
        PlayerMadeDecision += EndPlayerChoice; CharacterMove.PlayerHasReachedNextPoint += DeGatePlayerChoice;

    }

    void OnDisable() 
    {
        //Room Culling Events
        CharacterMove.PlayerMoved -= CullOldRooms; CharacterMove.PlayerMovingTowardsUnLoadedRoom -= PlayerArrivedToNewRoom; CharacterMove.PlayerWillClearList -= DestroyUnusedRooms;
        //On Rails Events
        handler.PlayerKilledAllZombies -= EncounterEnd;
        CharacterMove.PlayerHasReachedNextPoint -= PlayerToEncounterGate;
        //Player Events
        CharacterMove.PlayerHasReachedNextPoint -= EndRails;
        //Player Choice Events
        PlayerMadeDecision -= EndPlayerChoice; CharacterMove.PlayerHasReachedNextPoint -= DeGatePlayerChoice;
    }

    [ContextMenu("SpawnNewRoom")]
    public void CreateNewRoom()
    {
        var roomToSpawnNextTo = roomsQueue.Last();


        var newRoomData = NewRoomData();

        var newRoom = new Room(newRoomData);

       

        int randomDoor = UnityEngine.Random.Range((int)DoorHandler.DoorDirection.North, Enum.GetNames(typeof(DoorHandler.DoorDirection)).Length);

        DoorHandler.DoorDirection randomDoorDirection = (DoorHandler.DoorDirection)randomDoor;

        //Failsafe so rooms don't spawn into eachother
        if (randomDoorDirection != DoorHandler.DoorDirection.North)
        {

            if (randomDoorDirection == DoorHandler.DoorDirection.West && previousDirection == DoorHandler.DoorDirection.East || randomDoorDirection == DoorHandler.DoorDirection.East && previousDirection == DoorHandler.DoorDirection.West)
            {
                Debug.LogWarning("Avoiding Crossing rooms, re-rolling direction");
                List<DoorHandler.DoorDirection> otherTwoPossibilites = new List<DoorHandler.DoorDirection> { DoorHandler.DoorDirection.North, DoorHandler.DoorDirection.West, DoorHandler.DoorDirection.East };
                otherTwoPossibilites.Remove(previousDirection);

                randomDoorDirection = otherTwoPossibilites[UnityEngine.Random.Range(0, otherTwoPossibilites.Count)];
            }
        }


        Debug.LogWarning((DoorHandler.DoorDirection)randomDoor);


        previousDirection = (DoorHandler.DoorDirection)randomDoor;

        Vector3 spawnpoint = GetRoomsSpawnDiff(roomToSpawnNextTo, newRoom.roomPrefab, (DoorHandler.DoorDirection)randomDoor);

        var newlyCreatedRoomPrefab = Instantiate(newRoom.roomPrefab, spawnpoint, new Quaternion(roomToSpawnNextTo.transform.rotation.x, roomToSpawnNextTo.transform.rotation.y, roomToSpawnNextTo.transform.rotation.z, roomToSpawnNextTo.transform.rotation.w));

        spawnedRooms.Add(newRoom);

        roomsQueue.Enqueue(newlyCreatedRoomPrefab);

        Debug.Log("Create a room");
    }


    DoorHandler.DoorDirection previousDirection;
    Vector3 GetRoomsSpawnDiff(GameObject previousRoom, GameObject newRoom, DoorHandler.DoorDirection direction)
    {

        var previousRoomSize = RoomBounds(previousRoom).size;

        var newRoomSize = RoomBounds(newRoom).size;

        Vector3 vectorToReturn = new Vector3();

        float sizeOffset;


       

        //Check for which door
        switch (direction)
        {

            case (DoorHandler.DoorDirection.North):


                sizeOffset = previousRoomSize.z / 2 + newRoomSize.z / 2;
                vectorToReturn = new Vector3(previousRoom.transform.position.x, previousRoom.transform.position.y, previousRoom.transform.position.z + sizeOffset);

                break;
            case (DoorHandler.DoorDirection.East):

                sizeOffset = previousRoomSize.x / 2 + newRoomSize.x / 2;
                vectorToReturn = new Vector3(previousRoom.transform.position.x + sizeOffset, previousRoom.transform.position.y, previousRoom.transform.position.z);

                break;
            case (DoorHandler.DoorDirection.West):


                sizeOffset = previousRoomSize.x / 2 + newRoomSize.x / 2;
                vectorToReturn = new Vector3(previousRoom.transform.position.x - sizeOffset, previousRoom.transform.position.y, previousRoom.transform.position.z);

                break;

            case (DoorHandler.DoorDirection.South):

                Debug.LogError("Recieved South door! This makes no sense.");

                break;
        }


       
        previousDirection = (DoorHandler.DoorDirection)direction;

        return vectorToReturn;
    }

    Bounds RoomBounds(GameObject room)
    {
        var childRenderers = room.GetComponentsInChildren<Renderer>();

        if (childRenderers.Length == 0)
        {
            return room.GetComponent<Renderer>().bounds;
        }

        Bounds combined = childRenderers[0].bounds;

        for (int i = 1; i < childRenderers.Length; i++)
        {
            combined.Encapsulate(childRenderers[i].bounds);
        }

        return combined;
    }

   

   private RoomData NewRoomData()
    {

       var random =  UnityEngine.Random.Range(0,possibleRooms.Count);

       return possibleRooms[random];

    }



    [ContextMenu("Debug Get Size of the room")]
    private void DebugGetSize()
    {
        Debug.LogError(roomsQueue.Last().GetComponentInChildren<BoxCollider>().bounds.size);

    }

    private void CreateThreeWay(GameObject rootRoom)
    {
        if (currentThreeWay[1] != null)
        {
            previousThreeWay = currentThreeWay;
            Array.Clear(currentThreeWay,0,currentThreeWay.Length);
        }

        var numberOfRootRoomDoors = GameObject.FindGameObjectsWithTag("DoorTile").Where(x => x.transform.parent == rootRoom.transform).Count();

        int doorDirectionIndex = 0;

        int roomIndexer = 0;

        Room[] rooms = new Room[numberOfRootRoomDoors ];

        List<GameObject> roomsToQueue = new List<GameObject>();

        foreach (var room in rooms)
        {
            rooms[roomIndexer] = new Room(NewRoomData());
            

            if ( ((DoorHandler.DoorDirection)doorDirectionIndex == DoorHandler.DoorDirection.West && detected_swipe == SwipeDirection.Right))
            {
                doorDirectionIndex++;
                roomIndexer++;
                continue;
            }

            if (((DoorHandler.DoorDirection)doorDirectionIndex == DoorHandler.DoorDirection.East && detected_swipe == SwipeDirection.Left))
            {
                doorDirectionIndex++;
                roomIndexer++;
                continue;
            }

            if ( ((DoorHandler.DoorDirection)doorDirectionIndex == DoorHandler.DoorDirection.South))
            {
                doorDirectionIndex++;
                 roomIndexer++;
                 continue;
            }

            var newlyCreatedRoomPrefab = Instantiate(rooms[roomIndexer].roomPrefab, GetRoomsSpawnDiff(rootRoom, rooms[roomIndexer].roomPrefab, (DoorHandler.DoorDirection)doorDirectionIndex), new Quaternion(rootRoom.transform.rotation.x, rootRoom.transform.rotation.y, rootRoom.transform.rotation.z, rootRoom.transform.rotation.w));

            roomsToQueue.Add(newlyCreatedRoomPrefab);

            currentThreeWay[roomIndexer] = newlyCreatedRoomPrefab;

            roomIndexer++;

            doorDirectionIndex++;
        }


        Debug.Log($"Create three way with {rootRoom.name} as the base!");




    }

    
}
