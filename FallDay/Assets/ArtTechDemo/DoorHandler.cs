using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    // DoorHandler Needs a Rework, It is very rigid and Confusing. Basically it forces all rooms to have 4 doors leving all rooms very similar. (HardCode) _RLH107
    public bool debugisOn = false; // bool to control Debug Log.
    public enum DoorDirection
    {
    
        South,
        West,
        North,
        East
    }

    [Header("Set Doors (south door to element 0->west to 1->north to 2->east to 3)")]


    public GameObject[] doors;


}
