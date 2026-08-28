using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStalkerTrinket", menuName = "Trinket/StalkerTrinket")]
[Serializable]
public class StalkerTrinket : Trinket, IDamageFilterTrinket
{

    public List<Zombie> stalked_list = new List<Zombie>();

    public int stalked_damage_bonus;

    public int ModifiedDamage(int damage, Zombie target)
    {
        if (target.phase == Zombie.ZombiePhase.Far)
        {

            if (!stalked_list.Contains(target))
            {
                stalked_list.Add(target);
            }

            return 0;


        }

        else
        {
            if (stalked_list.Contains(target))
            {
                return damage + stalked_damage_bonus;
            }

            else
            {
                return damage;
            }
        }




    }


}
