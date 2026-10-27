using UnityEngine;

[CreateAssetMenu(fileName = "NewZombieDamageAndHealTrinket", menuName = "Trinket/Damage and Heal Trinket")]
public class DamageOrHealZombieEffectTrinket: ZombieEffectCarryingTrinket, IZombieModifyingTrinket
{

    public DamageOrHealZombieEffectTrinket()
    {
        effect = new TakeDamageAndHealZombieEffect();
    }

}