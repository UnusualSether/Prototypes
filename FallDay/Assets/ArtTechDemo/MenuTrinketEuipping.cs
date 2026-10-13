using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuTrinketEquipping : MonoBehaviour
{

    public UIDocument doc;

    public List<Trinket> available_trinkets_library;

    public VisualElement trinket_select_container;

    public Label currency_display;

    public string global_currency_total => MoneyHolder.total_soft_currency.total.ToString();

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

        trinket_display.Q<Toggle>("trinket_toggle").RegisterValueChangedCallback(evt => PassToEquipAndUnequip(trinket_to_display, trinket_display));

        if (GlobalTrinketHolder.player_chosen_trinkets.Contains(trinket_to_display))
        {
            trinket_display.Q<Toggle>("trinket_toggle").value = true;
        }

        if (trinket_to_display.acquired == false)
        {
            trinket_display.Q<Toggle>("trinket_toggle").visible = false;
            var buy_button = trinket_display.Q<Button>("buy_button");
            buy_button.visible = true;
            buy_button.RegisterCallback<ClickEvent>(evt => BuyTrinket(trinket_to_display, trinket_display));
        }

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

    

    void PassToEquipAndUnequip(Trinket toggled_trinket, VisualElement trinket_display)
    {

        if (GlobalTrinketHolder.player_chosen_trinkets.Contains(toggled_trinket))
        {
            UnequipTrinket(toggled_trinket);
        }
        else
        {
            if (GlobalTrinketHolder.IsEquipPossible(toggled_trinket) != GlobalTrinketHolder.EquipResult.Equippable)
            {
                trinket_display.Q<Toggle>("trinket_toggle").value = false;

                DisplayEquipFailureReason(GlobalTrinketHolder.IsEquipPossible(toggled_trinket));

                ShakeTrinketDisplay(trinket_display);

                return;
            }

            MinimizeTrinketDisplay(trinket_display);

            EquipTrinket(toggled_trinket);
        }
    }

    void DisplayEquipFailureReason(GlobalTrinketHolder.EquipResult reason)
    {
        if (reason == GlobalTrinketHolder.EquipResult.EquipLimitReached)
        {
            Debug.Log("Max trinket equip limit reached!");
        }

        if (reason == GlobalTrinketHolder.EquipResult.AlreadyEquipped)
        {
            Debug.Log("This is already equipped!");
        }
    }

    void EquipTrinket(Trinket trinket_to_send)
    {
        Debug.Log($"Attempting to equip {trinket_to_send.trinket_name}");

        GlobalTrinketHolder.TryEquip(trinket_to_send);
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

    void BuyTrinket(Trinket to_buy, VisualElement trinket_visual)
    {

        SoftCurrency to_charge = new SoftCurrency()
        {
            total = to_buy.trinket_cost
        };

        BuyBill trinket_bill = new BuyBill { charged_soft_currency = to_charge };


        if (MoneyHolder.Purchase(trinket_bill))
        {
            to_buy.AcquireTrinket();
            UpdateTrinketVisual(trinket_visual);
        }

        else
        {
            FailedToBuyTrinket(to_buy,trinket_visual);
        }
    }

    void FailedToBuyTrinket(Trinket to_buy, VisualElement display)
    {
        ShakeTrinketDisplay(display);
        Debug.Log("Couldn't afford the trinket!");
    }

    void UpdateTrinketVisual(VisualElement trinket_visual)
    {
        var buy_button = trinket_visual.Q<Button>("buy_button");

        buy_button.visible = false;

        trinket_visual.Q<Toggle>("trinket_toggle").visible = true;
    }


    public float trinket_visual_shake_magnitude;

    public float trinket_visual_shake_duration;

    public void ShakeTrinketDisplay(VisualElement display)
    {
        StartCoroutine(ShakeElement(display, trinket_visual_shake_duration, trinket_visual_shake_magnitude));
    }

    private IEnumerator ShakeElement(VisualElement elementToShake, float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Magnitude decreases over time for a natural effect
            float progress = elapsed / duration;
            float currentMagnitude = magnitude * (1f - progress);

            float randomX = UnityEngine.Random.Range(-currentMagnitude, currentMagnitude);
            float randomY = UnityEngine.Random.Range(-currentMagnitude, currentMagnitude);

            elementToShake.style.translate = new StyleTranslate(new Translate(randomX, randomY));

            elapsed += Time.deltaTime;
            yield return null;
        }

        elementToShake.style.translate = new StyleTranslate(StyleKeyword.None);
    }

    public float trinket_visual_minimize_magnitude;

    public float trinket_visual_minimize_duration;
    private void MinimizeTrinketDisplay(VisualElement display)
    {
        StartCoroutine(ElasticMinimizeElement(display, trinket_visual_minimize_duration, trinket_visual_minimize_magnitude));
    }

    private IEnumerator ElasticMinimizeElement(VisualElement element, float duration, float magnitude)
    {
        float elapsed_first = 0f;

        StyleScale beggining_scale = element.style.scale;

        while (elapsed_first < duration)
        {
            float progress = elapsed_first / duration;
            
            float current_magnitude = magnitude * (1f - progress);

            element.style.scale = new StyleScale(new Scale( new Vector2(current_magnitude,-current_magnitude) ) );

            elapsed_first += Time.deltaTime;
            yield return null;
        }

        element.style.scale = new StyleScale(StyleKeyword.None);
        

        
    }

}
