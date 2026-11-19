using UnityEngine.UIElements;

/// <summary>
/// Barra inferior + placa (bottom sheet) com três conteúdos: Config, Sound e Events.
/// Se a placa já está levantada, ela desce, troca o conteúdo e sobe de novo.
/// </summary>
public class BottomSheetPanel
{
    private enum Content { Config, Sound, Events }

    private readonly VisualElement _sheet;
    private readonly VisualElement _scrim;
    private readonly VisualElement _soundController;
    private readonly Button _openConfig;
    private readonly Button _openSound;
    private readonly Button _openEvents;
    private readonly Button _closeMenu;
    private readonly Button _start;
    private readonly LanguagePanel _language;
    private readonly long _transitionMs;

    private Content _pendingContent;
    private bool _swapPending;
    private IVisualElementScheduledItem _swapTask;

    public BottomSheetPanel(VisualElement root, UISoundManager sound, LanguagePanel language, long transitionMs)
    {
        _language = language;
        _transitionMs = transitionMs;

        _sheet = root.Q<VisualElement>(UINames.BottomSheet);
        _scrim = root.Q<VisualElement>(UINames.Scrim);
        _soundController = root.Q<VisualElement>(UINames.SoundController);
        _openConfig = root.Q<Button>(UINames.OpenConfig);
        _openSound = root.Q<Button>(UINames.OpenSound);
        _openEvents = root.Q<Button>(UINames.OpenEvents);
        _closeMenu = root.Q<Button>(UINames.CloseMenu);
        _start = root.Q<Button>(UINames.Play);
        var openShop = root.Q<Button>(UINames.OpenShop); // por enquanto só som

        // Estado inicial: scrim oculto
        if (_scrim != null) _scrim.style.display = DisplayStyle.None;

        _openConfig?.RegisterCallback<ClickEvent>(OnOpenConfig);
        _openSound?.RegisterCallback<ClickEvent>(OnOpenSound);
        _openEvents?.RegisterCallback<ClickEvent>(OnOpenEvents);
        _closeMenu?.RegisterCallback<ClickEvent>(OnClose);

        UIUtils.BindSound(sound, _openConfig, "Click");
        UIUtils.BindSound(sound, _openSound, "Click");
        UIUtils.BindSound(sound, _openEvents, "Click");
        UIUtils.BindSound(sound, openShop, "Click");
        UIUtils.BindSound(sound, _closeMenu, "Back");
    }

    private void OnOpenConfig(ClickEvent evt) => Open(Content.Config);
    private void OnOpenSound(ClickEvent evt) => Open(Content.Sound);
    private void OnOpenEvents(ClickEvent evt) => Open(Content.Events);

    private void Open(Content content)
    {
        if (_sheet == null) return;

        SetOverlay(true);

        // Cliques rápidos durante a descida: só atualiza o conteúdo que vai aparecer
        if (_swapPending)
        {
            _pendingContent = content;
            return;
        }

        bool isUp = _sheet.ClassListContains(UINames.BottomSheetUp);

        if (!isUp)
        {
            ApplyContent(content);
            _sheet.AddToClassList(UINames.BottomSheetUp);
            return;
        }

        // 1) desce
        _sheet.RemoveFromClassList(UINames.BottomSheetUp);
        _pendingContent = content;
        _swapPending = true;

        // 2) ao terminar de descer, troca o conteúdo e 3) sobe
        _swapTask = _sheet.schedule.Execute(() =>
        {
            _swapPending = false;
            ApplyContent(_pendingContent);
            _sheet.AddToClassList(UINames.BottomSheetUp);
        }).StartingIn(_transitionMs);
    }

    private void OnClose(ClickEvent evt)
    {
        _swapTask?.Pause();
        _swapPending = false;

        SetOverlay(false);
        _sheet?.RemoveFromClassList(UINames.BottomSheetUp);
    }

    // Define qual conteúdo aparece dentro da placa
    private void ApplyContent(Content content)
    {
        _language?.SetVisible(content == Content.Config);

        if (_soundController != null)
            _soundController.style.display = content == Content.Sound ? DisplayStyle.Flex : DisplayStyle.None;

        // Events: os dois ficam ocultos até você definir o conteúdo
    }

    // Fundo escurecido, botão de fechar e botão Start
    private void SetOverlay(bool on)
    {
        if (_scrim != null)
        {
            _scrim.style.display = on ? DisplayStyle.Flex : DisplayStyle.None;
            _scrim.EnableInClassList(UINames.ScrimFadeIn, on);
        }

        _start?.EnableInClassList(UINames.StartOff, on);
        _closeMenu?.EnableInClassList(UINames.CloseMenuOn, on);
    }

    public void Dispose()
    {
        _swapTask?.Pause();

        _openConfig?.UnregisterCallback<ClickEvent>(OnOpenConfig);
        _openSound?.UnregisterCallback<ClickEvent>(OnOpenSound);
        _openEvents?.UnregisterCallback<ClickEvent>(OnOpenEvents);
        _closeMenu?.UnregisterCallback<ClickEvent>(OnClose);
    }
}
