using UnityEngine;
using UnityEngine.UIElements;


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
        start = root.Q<Button>("StartGame");


        shotgun.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue == true)
            {
                pistol.SetValueWithoutNotify(false);
                choose.text = "Você escolheu a Shotgun";
                shotgun.value = true;
                start.SetEnabled(true);
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
            }
        });

    }

}
