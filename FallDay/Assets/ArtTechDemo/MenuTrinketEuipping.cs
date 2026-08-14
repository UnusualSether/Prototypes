using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem.Composites;
public class MenuTrinketEquipping : MonoBehaviour
{

    public UIDocument doc;

    public List<Trinket> available_trinkets_library;

    public Dictionary<Toggle, Trinket> button_to_trinket = new Dictionary<Toggle, Trinket>();

    private void Start()
    {

        Toggle vampiric_toggle = doc.rootVisualElement.Query<Toggle>("Vampiric");

        button_to_trinket.Add(vampiric_toggle, available_trinkets_library.First(h=> h is VampiricTrinket));

        vampiric_toggle.RegisterValueChangedCallback(evt => PassToEquipAndUnequip(vampiric_toggle));

        Toggle explorer_toggle = doc.rootVisualElement.Query<Toggle>("Explorer");

        button_to_trinket.Add(explorer_toggle, available_trinkets_library.First(h => h is ExplorersTrinket));

        explorer_toggle.RegisterValueChangedCallback(evt => PassToEquipAndUnequip(explorer_toggle));

        Toggle health_bonus_toggle = doc.rootVisualElement.Query<Toggle>("Umbilical");

        button_to_trinket.Add(health_bonus_toggle, available_trinkets_library.First(h => h is HealthAddingTrinket));

       health_bonus_toggle.RegisterValueChangedCallback(evt => PassToEquipAndUnequip(health_bonus_toggle));

    }


    

    void PassToEquipAndUnequip(Toggle key)
    {
        var toggled_trinket = button_to_trinket[key];

        if (GlobalTrinketHolder.player_chosen_trinkets.Contains(toggled_trinket))
        {
            UnequipTrinket(toggled_trinket);
        }
        else
        {
            EquipTrinket(toggled_trinket);
        }
    }

    void EquipTrinket(Trinket trinket_to_send)
    {
        Debug.Log($"Equipped {trinket_to_send.trinket_name}");
        GlobalTrinketHolder.player_chosen_trinkets.Add(trinket_to_send);
    }

    void UnequipTrinket(Trinket trinket_to_remove)
    {
        if (!GlobalTrinketHolder.player_chosen_trinkets.Contains(trinket_to_remove))
        {
            return;
        }

        Debug.Log($"Unequipped {trinket_to_remove.trinket_name}");

        GlobalTrinketHolder.player_chosen_trinkets.Remove(trinket_to_remove);
    }
}
