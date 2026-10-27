using System;
using System.Security.Cryptography;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;






public interface IOnTakeDamage
{
    public void TakeDamage(int amount_of_dmg, out bool should_zombie_take_damage, out int effect_on_total_damage)
    {
        should_zombie_take_damage = true;

        effect_on_total_damage = amount_of_dmg;
    }


}


public interface IOnDealDamage
{
    public void DealDamage()
    {
        
    }
}

public class TakeDamageAndHealZombieEffect : ZombieEffect , IOnTakeDamage
{

    enum HealOrDamage
    {
        Heal,
        TakeDamage
    }
    HealOrDamage heal_or_damage = HealOrDamage.TakeDamage;

    public void TakeDamage(int amount_of_dmg,out bool should_zombie_take_damage, out int effect_on_total_damage)
    {
        if (heal_or_damage == HealOrDamage.Heal)
        {
            should_zombie_take_damage = false;

            effect_on_total_damage = 0;

            attached_zombie.Heal(amount_of_dmg);

            Debug.Log("I'm on heal, now i should go to damage.");

            
        }

        else
        {
            should_zombie_take_damage = true;

            effect_on_total_damage = amount_of_dmg / 2;

            Debug.Log("I'm on damage, now i should go to heal.");

            
        }

        ChangeSwitch();

    }

    public void ChangeSwitch()
    {
        if (heal_or_damage == HealOrDamage.Heal)
        {
            heal_or_damage = HealOrDamage.TakeDamage;
        }

        else
        {
            heal_or_damage = HealOrDamage.Heal;
        }
    }
}

public class ExplodingZombieEffect : ZombieEffect, IOnDealDamage
{
    

    public void DealDamage()
    {
        GameHandler.PlayerTookDamage(attached_zombie.damage * 10);

        attached_zombie.Die();
    }

    
}

[Serializable]
public class ZombieEffect
{
    protected Zombie attached_zombie;


    public void SetAttachedZombie(Zombie target_zombie)
    {
        attached_zombie = target_zombie;
    }
    
}
