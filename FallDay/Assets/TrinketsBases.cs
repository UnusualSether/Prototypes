using UnityEngine;


/// <summary>
/// The base class for trinkets. Contains the trinkets name, description and icon. All `get` only.
/// </summary>
[System.Serializable]
public abstract class Trinket : ScriptableObject
{

    public string trinket_name { get; }

    public string trinket_description { get; }

    public Sprite trinket_icon { get; }

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



/// <summary>
/// Passive stat boost adding to the PlayerStats max hp.
/// </summary>
public class HealthAddingTrinket : Trinket, IPassiveTrinket
{

    int health_boost;

    void ApplyPassive(PlayerStats stats)
    {
        stats.max_hp += health_boost;
    }

}

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



/// <summary>
/// Kill effect trinkets which grant the player more current hp.
/// </summary>
public class VampiricTrinket : Trinket, IEventTricket
{

    public int health_gain;

    public void EventTrigger(TrinketEventType called_event_type, PlayerInstance instance_to_affect)
    {
        if (called_event_type == TrinketEventType.OnKill)
        {
            instance_to_affect.current_hp += health_gain;
        }
    }


}

/// <summary>
/// Trinkets which grant the player some kind of reward upon compeleting a room.
/// </summary>
public class RoomClearTrinket : Trinket, IEventTricket
{
    public void EventTrigger(TrinketEventType called_event_type, PlayerInstance instance_to_affect)
    {
        if (called_event_type == TrinketEventType.OnRoomComplete)
        {
            
        }
    }
}


