using UnityEngine;

[CreateAssetMenu(fileName = "NewMaxHealthBoostingReward", menuName = "RoomRewards/MaxHealth")]
public class MaxHealthBoostingReward: Reward
{

    public int amount_of_maxhp_granted;

    public MaxHealthBoostingReward()
    {
        reward_description = $"Increase your max health by {amount_of_maxhp_granted}";
    }
    public override void GainReward(PlayerInstance instance)
    {
        instance.stats.GainMaxHealth(amount_of_maxhp_granted);
    }

}
