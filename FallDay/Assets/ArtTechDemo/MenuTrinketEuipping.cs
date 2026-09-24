using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem.Composites;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class MenuTrinketEquipping : MonoBehaviour
{

    public UIDocument doc;

    public List<Trinket> available_trinkets_library;

    public VisualElement trinket_select_container;

    public VisualTreeAsset trinket_element_template;

    public Dictionary<Toggle, Trinket> button_to_trinket = new Dictionary<Toggle, Trinket>();

    private Dictionary<VisualElement, Trinket> activeTrinketDisplay = new Dictionary<VisualElement, Trinket>();

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += LanguageChange;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= LanguageChange;
    }

    private void Start()
    {

        var root = doc.rootVisualElement;

        trinket_select_container = root.Q<VisualElement>("trinket_container");


        BuildMenuOffData();

    }

    void LanguageChange(Locale newLocale)
    {
        RefreshText();
    }

    void ToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }



    void BuildMenuOffData()
    {
        foreach (var trinket in available_trinkets_library)
        {
            CreateNewTrinketSelector(trinket);
        }
    }

    void CreateNewTrinketSelector(Trinket trinket_to_display)
    {
        var trinket_display = trinket_element_template.Instantiate();

        var trinketimage = trinket_display.Q<Image>("trinket_sprite");

        TrinketVisual.IMG(trinketimage, trinket_to_display);

        //trinket_display.Q<Image>("trinket_sprite").sprite = trinket_to_display.trinket_sprite;

        trinket_display.Q<Label>("trinket_name").text = trinket_to_display.trinket_name;

        trinket_display.Q<Label>("trinket_desc").text = trinket_to_display.trinket_description;

        trinket_display.Q<Toggle>("trinket_toggle").RegisterValueChangedCallback(evt => PassToEquipAndUnequip(trinket_to_display));

        activeTrinketDisplay.Add(trinket_display, trinket_to_display);

        InsertInstantiatedIntoMain(trinket_display);
    }

    void RefreshText()
    {
        foreach (KeyValuePair<VisualElement, Trinket> pair in activeTrinketDisplay)
        {
            VisualElement TrinketName = pair.Key;
            Trinket trinket = pair.Value;

            TrinketName.Q<Label>("trinket_name").text = trinket.trinket_name;
        }
    }

    void InsertInstantiatedIntoMain(VisualElement element)
    {
        trinket_select_container.Add(element);
    }

    

    void PassToEquipAndUnequip(Trinket toggled_trinket)
    {

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
        GlobalTrinketHolder.ReceiveTrinketAdd(trinket_to_send);
    }

    void UnequipTrinket(Trinket trinket_to_remove)
    {
        if (!GlobalTrinketHolder.player_chosen_trinkets.Contains(trinket_to_remove))
        {
            throw new System.Exception("Global trinket holder does not contain this trinket! Whichever way you got to this is a bug.");
        }

        Debug.Log($"Unequipped {trinket_to_remove.trinket_name}");

        GlobalTrinketHolder.ReceiveTrinketRemove(trinket_to_remove);
    }
}
