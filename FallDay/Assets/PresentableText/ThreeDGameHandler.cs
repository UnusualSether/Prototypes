using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;


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
    public bool debugbool = false; // Place all debug Logs behind this bool, so you can easily turn them on and off. (Consider making it a public variable so you can change it in the inspector)

    #region Current, Previous and new Three ways

    //GameObject[] previousThreeWay = new GameObject[3];
    //GameObject[] currentThreeWay = new GameObject[3];
    //GameObject[] toDestroyThreeWay = new GameObject[3];


    GameObject[,] RoomsInternalGrid = new GameObject[5, 3];  //Grid in the 

    // The Rooms in The grid are Located and positioned In the following way:
    //      0.2 | 1.2 | 2.2 | 3.2 | 4.2
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
        //CharacterMove.PlayerMoved += CullOldRooms; 
        //CharacterMove.PlayerMoved += CullRoomList; 
        CharacterMove.PlayerMovingTowardsUnLoadedRoom += PlayerMovingTowardsNewRoom; 
        //CharacterMove.PlayerWillClearList += DestroyUnusedRooms; 
        //CharacterMove.PlayerHasReachedNextPoint += ClearPreviousRoomCache;
        //CharacterMove.PlayerHasReachedNextPoint += PlayerHasReachedNewRoom;
        //On Rails Events
        handler.PlayerKilledAllZombies += EncounterEnd;
        //Player Events
        CharacterMove.PlayerHasReachedNextPoint += PlayerToEncounterGate; 
        CharacterMove.PlayerHasReachedNextPoint += EndRails;
        //Player Choice Events
        PlayerMadeDecision += EndPlayerChoice;
        //PlayerSwipedOnChoice += CatchDirection;
        CharacterMove.PlayerHasReachedNextPoint += DeGatePlayerChoice;

    }

    void OnDisable() 
    {
        //Room Culling Events
        //CharacterMove.PlayerMoved -= CullOldRooms;
        CharacterMove.PlayerMovingTowardsUnLoadedRoom -= PlayerMovingTowardsNewRoom;
        //CharacterMove.PlayerWillClearList -= DestroyUnusedRooms;
        //CharacterMove.PlayerHasReachedNextPoint -= ClearPreviousRoomCache;
        //CharacterMove.PlayerHasReachedNextPoint -= PlayerHasReachedNewRoom;
        //On Rails Events
        handler.PlayerKilledAllZombies -= EncounterEnd;
        CharacterMove.PlayerHasReachedNextPoint -= PlayerToEncounterGate;
        //Player Events
        CharacterMove.PlayerHasReachedNextPoint -= EndRails;
        //Player Choice Events
        PlayerMadeDecision -= EndPlayerChoice;
        //PlayerSwipedOnChoice -= CatchDirection;
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
                if(debugbool) Debug.LogWarning("Avoiding Crossing rooms, re-rolling direction");
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
        if(debugbool) Debug.Log("Create a room");
    }


    DoorHandler.DoorDirection previousDirection;
    Vector3 GetRoomsSpawnDiff(GameObject previousRoom, GameObject newRoom, DoorHandler.DoorDirection direction)     // Mede o tamanho das salas para spawnar sem colidir Uma com a outra.
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
                if(debugbool) Debug.LogError("Recieved South door! This makes no sense.");
                break;
        }

        previousDirection = (DoorHandler.DoorDirection)direction;
        return vectorToReturn;
    }

    Bounds RoomBounds(GameObject room) // Mede tamanho da sala
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

   

    private RoomData NewRoomData() //Gerando Data Novo Para Spawn
    {
        var random =  UnityEngine.Random.Range(0,possibleRooms.Count);
        return possibleRooms[random];
    }

    private void VariateMovedGrid(int x, int y, out int newX, out int newY)     // Variates the movement of MoveGrid
    {
        switch (PlayerDirection)
        {
            default:
                // Move rooms north or Up // Not Needet, because Player can't moove south. Therefore, No need to move rooms north.
                newX = x;
                newY = y - 1;
                if(debugbool) Debug.LogWarning("Player Direction is None, no movement will occur. This should not happen, consider handling this case.");
                break;
            case SwipeDirection.Left:
                // Move rooms east or right x+1
                newX = x + 1;
                newY = y;
                if(debugbool) Debug.Log($"Moving room at ({x}, {y}) to ({newX}, {newY}) due to player moving left.");
                break;
            case SwipeDirection.Up:
                // Move rooms south or Down y-1
                newX = x;
                newY = y - 1;
                if(debugbool) Debug.Log($"Moving room at ({x}, {y}) to ({newX}, {newY}) due to player moving up.");
                break;
            case SwipeDirection.Right:
                // Move rooms west or left x-1
                newX = x - 1;
                newY = y;
                if(debugbool) Debug.Log($"Moving room at ({x}, {y}) to ({newX}, {newY}) due to player moving right.");
                break;
        }
    }


    SwipeDirection PlayerDirection; // Player direction: 0 for south, 1 for west, 2 for north, 3 for east

    // Catches the direction of the player's swipe and stores it in PlayerDirection
    // for use in moving the grid and generating new rooms in the correct positions
    public void CatchDirection(SwipeDirection direction)
    {
        PlayerDirection = direction;
        if(debugbool) Debug.Log($"Player Direction set to {PlayerDirection} _ThreeDGameHandler.main_CatchDirection");
        RoomsInternalGrid = GenerateMovedGrid();
        if(debugbool) DebugGrid();
    }
    private GameObject[,] GenerateMovedGrid() // Moves rooms in grid one position, in the direction opposing the player's movement
    {
        GameObject[,] ToReturn = new GameObject[5, 3];
        for (int x = 0; x < RoomsInternalGrid.GetLength(0); x++)
        {
            for (int y = 0; y < RoomsInternalGrid.GetLength(1); y++)
            {
                if (RoomsInternalGrid[x, y] != null)
                {
                    // Move the room to its new position based on the player's movement
                    int newX = 0, newY = 0;
                    VariateMovedGrid(x, y, out newX, out newY);    // Get the new position for the room based on the player's movement
                    if(newX >= 0 && newX < RoomsInternalGrid.GetLength(0) && newY >= 0 && newY < RoomsInternalGrid.GetLength(1))
                    {
                        ToReturn[newX, newY] = RoomsInternalGrid[x, y];
                    }
                    else
                    {
                        // If the new position is out of bounds, we destroy the room
                        // Uncomment the line below to enable debug logs for out of bounds rooms
                        if(debugbool) Debug.LogWarning($"Room at ({x}, {y}) moved out of bounds to ({newX}, {newY}). Not adding to new Array.");
                        Destroy(RoomsInternalGrid[x, y]);
                    }
                }
            }
        }
        return ToReturn;
    }   // Needs To generate New Grid Array to move Rooms Without risk of overiting rooms before they are moved, also needs to call GenerateNewRooms() after moving rooms.

    // The Rooms in The grid are Located and positioned In the following way:
    //      0.2 | 1.2 | 2.2 | 3.2 | 4.2
    //      0.1 | 1.1 | 2.1 | 3.1 | 4.1
    //      0.0 | 1.0 | 2.0 | 3.0 | 4.0


    private void SetNewRoomsInGridSwitch(GameObject Room, DoorHandler.DoorDirection doorDirectionIndex) // Sets the current grid to the new grid generated by GenerateMovedGrid()
    {
        switch (PlayerDirection) // Same switch condition as MovedGrid
        {
            default: // generate to Move rooms north or Up // if pos is not ocupyed by a room
                if (debugbool) Debug.LogWarning("Player Direction is None Or Does not Matsh the SetNewRoomsInGridSwitch, no movement will occur.");
                //3.1; 2.2; 1.1;
                //asdqa Shange this so that it works without Storein grid.
                RoomStorePos(Room);
                break;

            case SwipeDirection.Up:
                //3.1; 2.2; 1.1;
                RoomStorePos(Room);
                break;
            
            case SwipeDirection.Left:
                //3.1; 2.2;
                RoomStorePos(Room);
                break;
            
            case SwipeDirection.Right:
                // 2.2; 1.1;
                StorageShift++;
                RoomStorePos(Room);
                break;

        }
    }
    // This is used to determine where to generate new rooms in the grid after moving rooms
    // { It works Like a for but it is spread across many functions, it is set in CreateThreeWay before the foreach,
    // thogh SetNewRoomsInGridSwitch and used in  }
    private void RoomStorePos(GameObject Room)
    {
        switch (StorageShift)
        {
            case 0:
                if (RoomsInternalGrid[3, 1] == null)
                {
                    RoomsInternalGrid[3, 1] = Room;
                    StorageShift++;
                }
                break;
            case 1:
                if (RoomsInternalGrid[2, 2] == null)
                {
                    RoomsInternalGrid[2, 2] = Room;
                    StorageShift++;
                }
                break;
            case 2:
                if (RoomsInternalGrid[1, 1] == null)
                {
                    RoomsInternalGrid[1, 1] = Room;
                    StorageShift++;
                }
                break;
        }
    }

    private bool FirstThreeWayGenerated = true;
    private int StorageShift; //Changes StoregePos In RoomStorePos
    private void CreateThreeWay(GameObject rootRoom)
    {
        var numberOfRootRoomDoors = rootRoom.GetComponent<DoorHandler>().doors.Length;
        int doorDirectionIndex = 0;
        int roomIndexer = 0;
        Room[] rooms = new Room[numberOfRootRoomDoors];
        List<GameObject> roomsToQueue = new List<GameObject>();
        StorageShift = 0;

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
            if(debugbool) Debug.Log($"Creating room {roomIndexer} of three way with door direction {(DoorHandler.DoorDirection)doorDirectionIndex} and root room {rootRoom.name}");

            if(FirstThreeWayGenerated)
            {
                RoomsInternalGrid[2,1] = rootRoom; // Set the root room in the grid before generating new rooms, so we can check for it when generating new rooms and avoid overwriting it.
                FirstThreeWayGenerated = false;
            }

            var newlyCreatedRoomPrefab = 
                Instantiate(rooms[roomIndexer].roomPrefab,
                    GetRoomsSpawnDiff(      
                        rootRoom,
                        rooms[roomIndexer].roomPrefab,
                        (DoorHandler.DoorDirection)doorDirectionIndex),
                            new Quaternion(rootRoom.transform.rotation.x,
                                            rootRoom.transform.rotation.y,
                                            rootRoom.transform.rotation.z, 
                                            rootRoom.transform.rotation.w));

            //roomsToQueue.Add(newlyCreatedRoomPrefab);

            roomIndexer++;

            SetNewRoomsInGridSwitch(newlyCreatedRoomPrefab, (DoorHandler.DoorDirection)doorDirectionIndex);
            //currentThreeWay[roomIndexer - 1] = newlyCreatedRoomPrefab;
            //GenerateNewRooms();

            doorDirectionIndex++;
        }
        if (debugbool) Debug.Log($"Create three way with {rootRoom.name} as the base!");
        if (debugbool) DebugGrid();
    }


    // The Rooms in The grid are Located and positioned In the following way:
    //      0.2 | 1.2 | 2.2 | 3.2 | 4.2
    //      0.1 | 1.1 | 2.1 | 3.1 | 4.1
    //      0.0 | 1.0 | 2.0 | 3.0 | 4.0
    private void DebugGrid()
    {
        for (int x = 0; x < RoomsInternalGrid.GetLength(0); x++)
        {
            Debug.Log($"RoomsInternalGrid; " + ReturnforDebug(x, 0)+ ", "+ ReturnforDebug(x, 1)+", "+ ReturnforDebug(x, 2)+", "+ReturnforDebug(x, 3)+", "+ReturnforDebug(x, 4));
        }
    }

    private string ReturnforDebug(int x, int y)
    {
        if (RoomsInternalGrid[x, y] != null)
        {
            return $"{x}"+", "+ $"{y}";
        }
        else { return "null"; }
        
    }
}
