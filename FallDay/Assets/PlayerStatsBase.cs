using NUnit.Framework;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
/// <summary>
/// Contains stats which the PlayerInstance class requires for intialization.
/// </summary>
[System.Serializable]
public class PlayerStats
{

    public int max_hp;

    public int base_damage;


}


/// <summary>
/// The player instance, contains their current hp and damage output, a TrinketManager, SoftCurrency and PlayerStats.
/// </summary>
public class PlayerInstance
{

    public int current_hp;

    public int instance_damage;

    public PlayerStats stats;

    public TrinketManager trinket_manager;

    public SoftCurrency gained_currency;



    //PlayerInstance's ONLY connection to events.
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
        current_hp += gained_health;
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
    public void LoseHealth(int lost_health)
    {
        current_hp -= lost_health;
    }
    

    public PlayerInstance(PlayerStats stats)
    {
        trinket_manager.InitializeTrinketStatBoosts(stats);

        current_hp = stats.max_hp;

        instance_damage = stats.base_damage;
        
    }


    


}

/// <summary>
/// Basic Currency Abstract Class.
/// </summary>
[System.Serializable]
public abstract class Currency
{
    public int total;
}


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
    public List<Trinket> equipped_trinkets;

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
