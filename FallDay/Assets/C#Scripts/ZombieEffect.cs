using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;






public interface OnTakeDamage
{
    public void TakeDamage()
    {

    }


}


public interface OnDealDamage
{
    public void DealDamage()
    {

    }
}

public class ExplodingZombieEffect : ZombieEffect, OnDealDamage
{
    

    public void DealDamage()
    {
        GameHandler.PlayerTookDamage(attached_zombie.damage * 4);

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
