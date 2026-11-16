using System;
using UnityEngine.UIElements;

/// <summary>Janela Trinked: abre pelo botão Play e avança para o nível (via tutorial).</summary>
public class TrinkedPanel : UIPanel
{
    private readonly Button _play;
    private readonly Button _return;
    private readonly Button _openLevel;
    private readonly Action _onOpenLevel;

    public TrinkedPanel(VisualElement root, UISoundManager sound, Action onOpenLevel)
        : base(root, UINames.TrinkedWindow, UINames.TrinkedOn, UINames.TrinkedOff)
    {
        _onOpenLevel = onOpenLevel;

        _play = root.Q<Button>(UINames.Play);
        _return = root.Q<Button>(UINames.TrinkedReturn);
        _openLevel = root.Q<Button>(UINames.DifficultyButton);

        _play?.RegisterCallback<ClickEvent>(OnPlay);
        _return?.RegisterCallback<ClickEvent>(OnReturn);
        _openLevel?.RegisterCallback<ClickEvent>(OnOpenLevel);

        UIUtils.BindSound(sound, _play, "Confirm");
        UIUtils.BindSound(sound, _return, "Confirm");
        UIUtils.BindSound(sound, _openLevel, "Confirm");
    }

    private void OnPlay(ClickEvent evt) => Show();
    private void OnReturn(ClickEvent evt) => Hide();
    private void OnOpenLevel(ClickEvent evt) => _onOpenLevel?.Invoke();

    public override void Dispose()
    {
        base.Dispose();
        _play?.UnregisterCallback<ClickEvent>(OnPlay);
        _return?.UnregisterCallback<ClickEvent>(OnReturn);
        _openLevel?.UnregisterCallback<ClickEvent>(OnOpenLevel);
    }
}
