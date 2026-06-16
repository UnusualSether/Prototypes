using UnityEngine;
using static ThreeDGameHandler;

public class RoomHandler : MonoBehaviour
{
    public GameObject StartingRoom;
    public GameObject[,] rooms = new GameObject[3, 3];  // The grid of rooms, where 0,0 is the starting room, and 1,0 is the room to the right of the starting room, and so on.
    public GameObject[] RoomPool;
    public GameObject EndRoom;
    public ThreeDGameHandler threeDGameHandler;

    // Array of Rooms in System
    //      Rooms in the grid are represented as follows:
    //     0,2 | 1,2 | 2,2
    //     0,1 | 1,1 | 2,1
    //     0,0 | 1,0 | 2,0

    // The Rooms in The grig are Moved In the following way:

    //     the Room Selected is moved to position 1,1, and the other rooms are moved accordingly,
    //     for example, if the room selected is at position 1,2, then the room at position 1,1 is moved to position 1,0,
    //     and the room at position 2,1 is moved to position 2,0, and so on.
    //     Room Will Only be Generated in positions with y > 0 in positions 0,1, 1,2, 2,1

    //     Any Room that does not corespond to a grid pos is emedietly destroyed.
    //     Ex: Player selacts Room at position 2,1, the room at pos 0,1 will be destroyed, when he is to be repositioned.

    //     
    private bool StartupGate = false;

    private void OnEnable()
    {
        // ForEvents
    }
    private void OnDisable()
    {
        // Unsubscribe from Events
    }

    public void StartMovment()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //<= Generate Rooms in the grid With 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // This function is meant to generate a new Room once called.
    // Input: Door GameObject, which is the door that the player is going to enter.
    // Output: GameObject, which is the new room that is generated.
    private GameObject GenerateRoom() 
    {
        //<= Generate Room Logic Here
        return null;
    }
    // DoorHandler Needs a Rework, It is very rigid and Confusing. Basically it forces all rooms to have 4 doors leving all rooms very similar. (HardCode)


}
