using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class RoomDissipate : MonoBehaviour
{



    public float shrinkMagnitude;

    public float shrinkDuration;


    public void StartRoomDestroyCoroutine()
    {
        StartCoroutine(RoomShrink(shrinkMagnitude, shrinkDuration));

        Debug.Log("Started Dissapate!");
    }
   
    IEnumerator RoomShrink(float magnitude, float time)
    {


        var shrinkVector = new Vector3(magnitude, magnitude, magnitude);

        while (gameObject.transform.localScale != Vector3.zero)
        {
            gameObject.transform.localScale -= shrinkVector;


            yield return null;
        }

        EndRoom();
    }

    void EndRoom()
    {
        Destroy(gameObject);
    }
}
