using UnityEngine;
using UnityEngine.UIElements;
using static GameHandler;
using static Weapon;


public partial class WeaponSelection : MonoBehaviour
{
    private Toggle shotgun, pistol;
    private Label choose;
    private Button start;

    private void Start()
    {
        start.SetEnabled(false);
    }

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        shotgun = root.Q<Toggle>("Shotgun");
        pistol = root.Q<Toggle>("Pistol");
        choose = root.Q<Label>("ChooseWeapon");
        start = root.Q<Button>("ToDifficulty");


        shotgun.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue == true)
            {
                pistol.SetValueWithoutNotify(false);
                choose.text = "Você escolheu a Shotgun";
                shotgun.value = true;
                start.SetEnabled(true);
            }
            else if(evt.newValue == false && pistol.value == false)
            {
                choose.text = "Escolha uma arma";
                start.SetEnabled(false);
            }
        });


        pistol.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue == true)
            {
                shotgun.SetValueWithoutNotify(false);
                choose.text = "Você escolheu a Pistola";
                pistol.value = true;
                start.SetEnabled(true);
                Weapon.GameHandler.currentWeapon == new Pistol();
            }
            else if (evt.newValue == false && pistol.value == false)
            {
                choose.text = "Escolha uma arma";
                start.SetEnabled(false);
            }
        });

    }

    private void AllowStart()
    {
        if(shotgun.value == true || pistol.value == true)
        {
            start.SetEnabled(true);
        }
        else
        {
            start.SetEnabled(false);
        }
            
    }

}
