using System.ComponentModel;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class DoorHandler : MonoBehaviour
{
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
