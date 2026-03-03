using UnityEngine;

public class RoomView : MonoBehaviour
{
    public RoomInstance roomInstance { get; private set; }

    public void Initialize(RoomInstance instance)
    {
        roomInstance = instance;

        Debug.Log($"Entered {instance.data.RoomName}");
    }


}
