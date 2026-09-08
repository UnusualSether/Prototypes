using UnityEngine;

public class RoomTileOverlapPrevention : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var childObject in gameObject.transform.GetComponentsInChildren<Transform>())
        {
            if (childObject.CompareTag("Waypoint"))
            {
                continue;
            }

            else
            {
               
            }

        }

        
    }
}
