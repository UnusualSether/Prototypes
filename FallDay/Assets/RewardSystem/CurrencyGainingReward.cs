using UnityEngine;

[CreateAssetMenu(fileName = "NewCurrencyReward", menuName = "RoomRewards/Currency")]
public class CurrencyGainingReward: Reward
{
    public int currency_gained;

    public CurrencyGainingReward()
    {
        reward_description = $"Gain {currency_gained} currency.";
    }

    public override void GainReward(PlayerInstance instance)
    {
        instance.GainSoftCurrency(currency_gained);
    }
}
