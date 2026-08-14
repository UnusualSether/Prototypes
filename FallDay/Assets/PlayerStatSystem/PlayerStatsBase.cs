using NUnit.Framework;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting;
using System.Security.Authentication.ExtendedProtection;
/// <summary>
/// Contains stats which the PlayerInstance class requires for intialization.
/// </summary>
/// 



[System.Serializable]
public class PlayerStats
{

    public float max_hp;

    public int base_damage;

    
    public PlayerStats()
    {
        max_hp = 100;

        base_damage = 1;
    }
        
    


}


/// <summary>
/// The player instance, contains their current hp and damage output, a TrinketManager, SoftCurrency and PlayerStats.
/// </summary>
[System.Serializable]
public class PlayerInstance
{

    public GameHandler player_handler;

    public PlayerStats stats = new PlayerStats();

    public float current_hp;

    public int instance_damage;

    public TrinketManager trinket_manager = new TrinketManager();

    public SoftCurrency gained_currency = new SoftCurrency();


 
    void PlayerKilledZombie()
    {
        DispatchEvent(TrinketEventType.OnKill);
    }

    void PlayerClearedRoom()
    {
        DispatchEvent(TrinketEventType.OnRoomComplete);
    }

    void PlayerTookDamageRoom()
    {

    }


    void DispatchEvent(TrinketEventType event_type)
    {
        trinket_manager.IdentifyEventType(event_type, this);
    }
    /// <summary>
    /// Adds to the players current health points.
    /// </summary>
    /// <param name="gained_health"></param>
    public void GainHealth(int gained_health)
    {
        if (PlayerFullHP())
        {
            return;
        }

        current_hp += gained_health;
    }

    public bool PlayerFullHP()
    {
        if (current_hp == stats.max_hp)
        {
            return true;
        }

        else
        {
            return false;
        }
    }


    /// <summary>
    /// Sets the players current hp to the PlayerStats' max amount.
    /// </summary>
    public void MaxOutHealth()
    {
        current_hp = stats.max_hp;
    }

    /// <summary>
    /// Deducts from the PlayerInstance's current health.
    /// </summary>
    /// <param name="lost_health"></param>
    public void LoseHealth(float lost_health)
    {
        if (DeathCheck(lost_health))
        {
            PlayerDied();
        }
        else
        {
            current_hp -= lost_health;
        }
    }

    public void PlayerDied()
    {
        Debug.Log("I Died!");
    }

    public bool DeathCheck(float damage)
    {
        if (current_hp - damage <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    
    public void GainSoftCurrency(int amount_gained)
    {
        gained_currency.Add(amount_gained);
    }

    private protected void SetOwnStats()
    {
        current_hp = stats.max_hp;

        instance_damage = stats.base_damage; 
    }

    public PlayerInstance(GameHandler initialized_by)
    {
        player_handler = initialized_by;


        player_handler.ZombieKilled += PlayerKilledZombie;

        player_handler.PlayerKilledAllZombies += PlayerClearedRoom;

        trinket_manager.equipped_trinkets = GlobalTrinketHolder.player_chosen_trinkets;

        SetOwnStats();

        trinket_manager.InitializeTrinketStatBoosts(stats);



        GameHandler.PlayerTookDamage += LoseHealth;        
    }

    
    


}

/// <summary>
/// Basic Currency Abstract Class.
/// </summary>
[System.Serializable]
public abstract class Currency
{
    public int total;

    public void Add(int amount_added)
    {
        total += amount_added;
    }

    public void Deduct(int amount_deducted)
    {
        total -= amount_deducted;
    }
}

[System.Serializable]
/// <summary>
/// Free Currency. Currently only contains a total int var.
/// </summary>
public class SoftCurrency : Currency
{

}

/// <summary>
/// Premium Currency. Simple now, simpyl for future planning.
/// </summary>
public class PremiumCurrency: Currency
{

}

/// <summary>
/// Contains the currently equipped trinkets and handles giving the player their respective stat boots and event gains off said trinkets.
/// </summary>
[System.Serializable]
public class TrinketManager
{
    public List<Trinket> equipped_trinkets = new List<Trinket>();

    /// <summary>
    /// Recieves the PlayerInstance's Dispatch function. Finds out what event to call depending of the received event type.
    /// </summary>
    /// <param name="event_type"></param>
    /// <param name="instance"></param>
    public void IdentifyEventType(TrinketEventType event_type, PlayerInstance instance)
    {
        if (event_type == TrinketEventType.OnKill)
        {
            DispatchOnKillEffects(instance);
        }

        if (event_type == TrinketEventType.OnRoomComplete)
        {
            DispatchOnRoomClearedEffects(instance);
        }
    }


    /// <summary>
    /// Called whenever a PlayerInstance is created so trinket stat boosts can be added.
    /// </summary>
    /// <param name="stats_to_affect"></param>
    public void InitializeTrinketStatBoosts(PlayerStats stats_to_affect)
    {

        foreach (var trinket in equipped_trinkets)
        {
            if (trinket is IPassiveTrinket passive_trinket)
            {
                passive_trinket.ApplyPassive(stats_to_affect);
            }
        }
    }


    /// <summary>
    /// Calls alls OnKill trinket effects.
    /// </summary>
    /// <param name="instance"></param>
    public void DispatchOnKillEffects(PlayerInstance instance)
    {
        foreach (var trinket in equipped_trinkets)
        {
            if (trinket is IEventTricket event_tricket)
            {
                event_tricket.EventTrigger(TrinketEventType.OnKill, instance);
            }
        }
    }

    /// <summary>
    /// Calls all OnRoomComplete trinket effects.
    /// </summary>
    /// <param name="instance"></param>
    void DispatchOnRoomClearedEffects(PlayerInstance instance)
    {
        foreach (var trinket in equipped_trinkets)
        {
            if (trinket is IEventTricket event_tricket)
            {
                event_tricket.EventTrigger(TrinketEventType.OnRoomComplete,instance);
            }
        }
    }

    /// <summary>
    /// Adds a trinket to the equipped trinkets array.
    /// </summary>
    /// <param name="added_trinket"></param>
    void AddTrinket(Trinket added_trinket)
    {
        equipped_trinkets.Append(added_trinket);
    }

    /// <summary>
    /// Removes a trinket from the equipped trinkets array.
    /// </summary>
    /// <param name="removed_trinket"></param>
    void RemoveTrinket(Trinket removed_trinket)
    {
        equipped_trinkets.Remove(removed_trinket);
    }

   
}

