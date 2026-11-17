using UnityEngine.UIElements;

 
public class HapticsSettingsPanel
{
    private readonly Toggle _toggle;

    public HapticsSettingsPanel(VisualElement root)
    {
        _toggle = root.Q<Toggle>(UINames.HapticsToggle);
        if (_toggle == null) return;

        _toggle.SetValueWithoutNotify(Haptics.Enabled);
        _toggle.RegisterValueChangedCallback(OnChanged);
    }

    private void OnChanged(ChangeEvent<bool> evt)
    {
        Haptics.Enabled = evt.newValue;

        // Ao ligar, vibra uma vez para confirmar
        if (evt.newValue) Haptics.Play(HapticStrength.Light);
    }

    public void Dispose()
    {
        _toggle?.UnregisterValueChangedCallback(OnChanged);
    }
}