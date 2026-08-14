using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;



public static class GlobalTrinketHolder
{
    public static List<Trinket> player_chosen_trinkets = new List<Trinket>();
}

#region Trinket Base and Interfaces
/// <summary>
/// The base class for trinkets. Contains the trinkets name, description and icon. All `get` only.
/// </summary>
[System.Serializable]
public class Trinket : ScriptableObject
{

    [SerializeField] protected string _trinket_name;
    public string trinket_name { get => _trinket_name; set => _trinket_name = value; }

    [SerializeField] protected string _trinket_description;
    public string trinket_description { get => _trinket_description; set => _trinket_description = value; }

    [SerializeField] protected Sprite _trinket_sprite;
    public Sprite trinket_sprite { get => _trinket_sprite; set => _trinket_sprite = value; }

}

/// <summary>
/// USed to distinguish which trinkets should activate on which effects.
/// </summary>
public enum TrinketEventType
{
    OnKill,
    OnRoomComplete,
    OnTakeDamage
}


/// <summary>
/// Interface for all trinkets which activate upon an in-game event occuring. (Room cleared, enemy defeated, etc.)
/// </summary>
public interface IEventTricket
{

   
    void EventTrigger(TrinketEventType called_event_type, PlayerInstance instance_to_affect) { }
}


/// <summary>
/// Interface for all trinkets which apply passive stat boosts to the player's PlayerStats class.
/// </summary>
public interface IPassiveTrinket
{


    void ApplyPassive(PlayerStats stats) { }
}


#endregion

#region Trinket Type Bases


/// <summary>
/// Passive stat boost adding to the PlayerStats base damage.
/// </summary>
public class DamageAddingTrinket : Trinket, IPassiveTrinket
{

    int damage_boost;

    void ApplyPassive(PlayerStats stats)
    {
        stats.base_damage += damage_boost;
    }

}


public class OnKillTrinket : Trinket, IEventTricket
{

    public virtual void EventTrigger(TrinketEventType called_event_type, PlayerInstance instance_to_affect)
    {
        if (called_event_type == TrinketEventType.OnKill)
        {
            
        }
    }

}


/// <summary>
/// Trinkets which grant the player some kind of reward upon compeleting a room.
/// </summary>
public class RoomClearTrinket : Trinket, IEventTricket
{
    public virtual void EventTrigger(TrinketEventType called_event_type, PlayerInstance instance_to_affect)
    {
        if (called_event_type == TrinketEventType.OnRoomComplete)
        {

        }
    }
}

#endregion


 




