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

    //GameObject[] previousThreeWay = new GameObject[3];
    //GameObject[] currentThreeWay = new GameObject[3];
    //GameObject[] toDestroyThreeWay = new GameObject[3];
    
    
    GameObject[,] RoomsInternalGrid = new GameObject[5, 3];  //Grid in the 

    // The Rooms in The grid are Located and positioned In the following way:
    //      0.3 | 1.3 | 2.3 | 3.3 | 4.3
    //      0.1 | 1.1 | 2.1 | 3.1 | 4.1
    //      0.0 | 1.0 | 2.0 | 3.0 | 4.0

    //     The Room at position 2.1 is the room the player is currently in, and the rooms at positions 1.1 and 3.1 are the rooms that the player can choose to go to next, 
    
    //     Any Room that does not corespond to a grid pos is emedietly destroyed.

    //     For example, if the player is in room 2.1 and chooses to go to room 3.1, movement to the right,
    //          the 3.1 will be moved to pos 2.1
    //          This Movement will de Repeated by all members of the grid
    //          
    //          

    // These Rules are controled by the Following Functions:

    #endregion


    /// This is a function which you can call on any other partial ThreeDGameHandler script, it is basically a 
    ///start which doesn't cause conflict between partial classes. Basically, use this instead of Start().
    partial void OnStartExtendRoomCulling();

    partial void OnStartExtendRailMethod();

    partial void OnStartExtendPlayerChoiceMethod();

    private void Start()
    {
        OnStartExtendRailMethod();
        OnStartExtendRoomCulling();
        OnStartExtendPlayerChoiceMethod();
    }

    void Update() //Update?
    {
        HandleSwipeInput();
    }

    void OnEnable() 
    { 
        //Room Culling Events
        CharacterMove.PlayerMoved += CullOldRooms; 
        CharacterMove.PlayerMoved += CullRoomList; 
        CharacterMove.PlayerMovingTowardsUnLoadedRoom += PlayerMovingTowardsNewRoom; 
        CharacterMove.PlayerWillClearList += DestroyUnusedRooms; 
        //CharacterMove.PlayerHasReachedNextPoint += ClearPreviousRoomCache;
        CharacterMove.PlayerHasReachedNextPoint += PlayerHasReachedNewRoom;
        //On Rails Events
        handler.PlayerKilledAllZombies += EncounterEnd;
        //Player Events
        CharacterMove.PlayerHasReachedNextPoint += PlayerToEncounterGate; CharacterMove.PlayerHasReachedNextPoint += EndRails;
        //Player Choice Events
        PlayerMadeDecision += EndPlayerChoice;
        CharacterMove.PlayerHasReachedNextPoint += DeGatePlayerChoice;

    }

    void OnDisable() 
    {
        //Room Culling Events
        CharacterMove.PlayerMoved -= CullOldRooms;
        CharacterMove.PlayerMovingTowardsUnLoadedRoom -= PlayerMovingTowardsNewRoom;
        CharacterMove.PlayerWillClearList -= DestroyUnusedRooms;
        //CharacterMove.PlayerHasReachedNextPoint -= ClearPreviousRoomCache;
        CharacterMove.PlayerHasReachedNextPoint -= PlayerHasReachedNewRoom;
        //On Rails Events
        handler.PlayerKilledAllZombies -= EncounterEnd;
        CharacterMove.PlayerHasReachedNextPoint -= PlayerToEncounterGate;
        //Player Events
        CharacterMove.PlayerHasReachedNextPoint -= EndRails;
        //Player Choice Events
        PlayerMadeDecision -= EndPlayerChoice;
        CharacterMove.PlayerHasReachedNextPoint -= DeGatePlayerChoice;
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




    SwipeDirection PlayerDirection; // Player direction: 0 for south, 1 for west, 2 for north, 3 for east


    // Catches the direction of the player's swipe and stores it in PlayerDirection for use in moving the grid and generating new rooms in the correct positions
    // is in Event PlayerSwipedOnChoice
    public void CatchDirection(SwipeDirection direction)    
    {
        PlayerDirection = direction;
    }
    private void MovedGrid()
    {
        switch (PlayerDirection)
        {
            case 0:
                // Move rooms north or Up
                break;
            case 1:
                // Move rooms east or right
                break;
            case 2:
                // Move rooms south or Down
                break;
            case 3:
                // Move rooms west or left
                break;
        }
    }

    private GameObject[,] GenerateMovedGrid() // Moves rooms in grid one position in the direction opposite of the player's movement
    {
        GameObject[,] ToReturn = new GameObject[5, 3];
        for (int x = 0; x < RoomsInternalGrid.GetLength(0); x++)
        {
            for (int y = 0; y < RoomsInternalGrid.GetLength(1); y++)
            {
                if (RoomsInternalGrid[x, y] != null)
                {
                    // Move the room to its new position based on the player's movement
                    
                    
                }
            }
        }
        return ToReturn;
    }   // Needs To generate New Grid Array to move Rooms Without risk of overiting rooms before they are moved, also needs to call GenerateNewRooms() after moving rooms.









    private void CreateThreeWay(GameObject rootRoom)
    {
        var numberOfRootRoomDoors = rootRoom.GetComponent<DoorHandler>().doors.Length;
        int doorDirectionIndex = 0;
        int roomIndexer = 0;
        Room[] rooms = new Room[numberOfRootRoomDoors];
        List<GameObject> roomsToQueue = new List<GameObject>();

        foreach (var room in rooms)
        {
            rooms[roomIndexer] = new Room(NewRoomData());
            if ((DoorHandler.DoorDirection)doorDirectionIndex == DoorHandler.DoorDirection.West && detected_swipe == SwipeDirection.Right)
            {
                doorDirectionIndex++;
                //roomIndexer++;
                continue;
            }
            if ((DoorHandler.DoorDirection)doorDirectionIndex == DoorHandler.DoorDirection.East && detected_swipe == SwipeDirection.Left)
            {
                doorDirectionIndex++;
                //roomIndexer++;
                continue;
            }
            if ((DoorHandler.DoorDirection)doorDirectionIndex == DoorHandler.DoorDirection.South)
            {
                doorDirectionIndex++;
                 //roomIndexer++;
                 continue;
            }
            var newlyCreatedRoomPrefab = Instantiate(rooms[roomIndexer].roomPrefab, GetRoomsSpawnDiff(rootRoom, rooms[roomIndexer].roomPrefab, (DoorHandler.DoorDirection)doorDirectionIndex), new Quaternion(rootRoom.transform.rotation.x, rootRoom.transform.rotation.y, rootRoom.transform.rotation.z, rootRoom.transform.rotation.w));

            roomsToQueue.Add(newlyCreatedRoomPrefab);

            roomIndexer++;

            currentThreeWay[roomIndexer - 1] = newlyCreatedRoomPrefab;
            GenerateNewRooms();

            doorDirectionIndex++;
        }
        Debug.Log($"Create three way with {rootRoom.name} as the base!");
    }

    /// <summary>
    /// Triggered by CharacterMove.PlayerHasReachedNextPoint
    /// Copies old three way rooms to the previous threeWay array and destroys them as soon
    /// as the player reaches a new room.
    /// </summary>
    /*
    void ClearPreviousRoomCache()
    {


        if (currentThreeWay[1] != null) 
        {
            previousThreeWay = currentThreeWay;


            Array.Clear(currentThreeWay, 0, currentThreeWay.Length);


            foreach (var previousRoom in previousThreeWay)
            {
                if (previousRoom != null)
                {
                    DestroyRoom(previousRoom);
                }
            }
            Array.Clear(previousThreeWay, 0, currentThreeWay.Length);
        }
    }
    */

}
