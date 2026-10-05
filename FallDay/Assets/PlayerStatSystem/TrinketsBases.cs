using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.Localization.Settings;
using Unity.VisualScripting;



public static class GlobalTrinketHolder
{
    public static List<Trinket> player_chosen_trinkets = new List<Trinket>();

    public static int max_trinkets = 2;

    public static void ReceiveTrinketAdd(Trinket recieved_trinket)
    { 

        player_chosen_trinkets.Add(recieved_trinket);
    }

    public static void ReceiveTrinketRemove(Trinket remove_request_trinket)
    {
        player_chosen_trinkets.Remove(remove_request_trinket);
    }

    public enum EquipResult
    {
        Equippable,
        EquipLimitReached,

        AlreadyEquipped
    }

    public static EquipResult IsEquipPossible(Trinket recieved_)
    {
        if (player_chosen_trinkets.Count >= max_trinkets)
        {
            return EquipResult.EquipLimitReached;
        }

        if (player_chosen_trinkets.Contains(recieved_))
        {
            return EquipResult.AlreadyEquipped;
        }

        else
        {
            return EquipResult.Equippable;
        }
    }

    public static bool TryEquip(Trinket recieved_)
    {
        if (player_chosen_trinkets.Count >= max_trinkets)
        {
            return false;
        }

        if (player_chosen_trinkets.Contains(recieved_))
        { 
            return false;
        }

        else
        {
            ReceiveTrinketAdd(recieved_);
            return true;
        }


    }
}

#region Trinket Base and Interfaces
/// <summary>
/// The base class for trinkets. Contains the trinkets name, description and icon. All `get` only.
/// </summary>
[System.Serializable]
public class Trinket : ScriptableObject
{

    [SerializeField] protected string _trinket_name;
    [SerializeField] protected string _trinket_namePTBR;
    public string trinket_name
    {
        get
        {
            string currentLocale = LocalizationSettings.SelectedLocale.Identifier.Code;

            if (currentLocale.StartsWith("pt"))
            {
                return _trinket_namePTBR;
            }
            else
            {
                return _trinket_name;
            }
        }
        set => _trinket_name = value;
    }


    [SerializeField] protected string _trinket_description;
    public string trinket_description { get => _trinket_description; set => _trinket_description = value; }

    [SerializeField] protected Sprite[] _trinket_sprite;
    public Sprite[] trinket_sprite => _trinket_sprite;

    [SerializeField] protected bool _acquired;
    public bool acquired { get => _acquired; set => _acquired = value; }

    [SerializeField] protected int _trinket_cost;
    public int trinket_cost { get => _trinket_cost; set => trinket_cost = value;  }

    public void AcquireTrinket()
    {
        _acquired = true;
    }

}

    /// <summary>
    /// USed to distinguish which trinkets should activate on which effects.
    /// </summary>
    public enum TrinketEventType
{
    OnKill,
    OnRoomComplete,
    OnTakeDamage,
    NewEncounterPulled
}


/// <summary>
/// Interface for all trinkets which activate upon an in-game event occuring. (Room cleared, enemy defeated, etc.)
/// </summary>
public interface IEventTricket
{

   
    void EventTrigger(TrinketEventType called_event_type, PlayerInstance instance_to_affect) { }
}

public interface IDamageFilterTrinket
{


    int ModifiedDamage(int damage, Zombie target);
}


/// <summary>
/// Interface for all trinkets which apply passive stat boosts to the player's PlayerStats class.
/// </summary>
public interface IPassiveTrinket
{


    void ApplyPassive(PlayerStats stats) { }
}


public interface IPullInfoFromEncounterTrinket
{


    void PullEncounterInfo(EncounterData encounter_data);
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


 




