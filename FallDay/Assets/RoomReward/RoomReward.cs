using UnityEngine;


[CreateAssetMenu(fileName = "NewRoomReward", menuName = "RoomReward/Reward")]
public class RoomReward: ScriptableObject
{


    public string reward_name;



    public void TakeReward(PlayerStats receiver)
    {
        Debug.Log($"Received {reward_name}");
    }
}
