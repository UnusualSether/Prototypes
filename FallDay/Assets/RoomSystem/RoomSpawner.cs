using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public Transform[] roomAnchor;

    public int roomAnchorIndex;


       public void SpawnRoom(RoomInstance instance)
    {
        GameObject go = Instantiate(instance.data.prefab, roomAnchor[roomAnchorIndex]);
        roomAnchorIndex = (roomAnchorIndex + 1) % roomAnchor.Length;
        go.GetComponent<RoomView>().Initialize(instance);

    }
}
