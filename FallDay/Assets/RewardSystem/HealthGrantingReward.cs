using UnityEngine;

[CreateAssetMenu(fileName = "NewHealthReward", menuName = "RoomRewards/HealthGain")]
public class HealthGrantingReward : Reward
{

    public int health_gain;

    public HealthGrantingReward()
    {
        reward_description = $"Recover {health_gain} health.";
    }

    public override void GainReward(PlayerInstance player)
    {
        player.GainHealth(health_gain);
    }


}
