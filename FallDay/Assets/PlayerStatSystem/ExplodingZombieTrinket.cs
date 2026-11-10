using UnityEngine;

[CreateAssetMenu(fileName = "NewZombieExplodingTrinket", menuName = "Trinket/Zombie Exploding Trinket")]
public class ExplodingZombieTrinket : ZombieEffectCarryingTrinket, IZombieModifyingTrinket
{

    public ExplodingZombieTrinket()
    {
        effect = new ExplodingZombieEffect();
    }

}
