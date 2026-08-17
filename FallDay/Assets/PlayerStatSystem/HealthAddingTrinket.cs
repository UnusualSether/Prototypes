using UnityEngine;

[CreateAssetMenu(fileName = "NewHealthAddingTrinket", menuName = "Trinket/Health Bonus Trinket")]
public class HealthAddingTrinket : Trinket, IPassiveTrinket
{

    public int health_boost;

    public void ApplyPassive(PlayerStats stats)
    {
        stats.ChangeMaxHealth(health_boost);
    }

}