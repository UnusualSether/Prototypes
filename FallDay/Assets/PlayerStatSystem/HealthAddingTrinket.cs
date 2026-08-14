using UnityEngine;

[CreateAssetMenu(fileName = "NewHealthAddingTrinket", menuName = "Trinket/Health Bonus Trinket")]
public class HealthAddingTrinket : Trinket, IPassiveTrinket
{

    public int health_boost;

    void ApplyPassive(PlayerStats stats)
    {
        stats.max_hp += health_boost;
    }

}