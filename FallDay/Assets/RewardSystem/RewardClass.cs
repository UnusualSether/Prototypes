using UnityEngine;


public class Reward: ScriptableObject
{
    public string reward_name;

    public string reward_description;

    public Sprite reward_sprite;
    public virtual void GainReward(PlayerInstance instance)
    {
        Debug.Log($"Got {reward_name}");
    }



}
