using UnityEngine.UIElements;

/// <summary>
/// Base para janelas que entram/saem com classes USS:
/// Show  -> display flex + classe "on"
/// Hide  -> classe "off" e, ao fim da transição, display none
/// </summary>
public abstract class UIPanel
{
    protected readonly VisualElement Root;
    private readonly string _onClass;
    private readonly string _offClass;

    protected UIPanel(VisualElement root, string elementName, string onClass, string offClass)
    {
        Root = root.Q<VisualElement>(elementName);
        _onClass = onClass;
        _offClass = offClass;

        Root?.RegisterCallback<TransitionEndEvent>(OnTransitionEnd);
    }

    public virtual void Show()
    {
        if (Root == null) return;

        Root.style.display = DisplayStyle.Flex;
        Root.RemoveFromClassList(_offClass);
        Root.schedule.Execute(() => Root.AddToClassList(_onClass));
    }

    public virtual void Hide()
    {
        if (Root == null) return;

        Root.RemoveFromClassList(_onClass);
        Root.AddToClassList(_offClass);
    }

    // Só reage à transição da própria janela (ignora transições de elementos filhos)
    private void OnTransitionEnd(TransitionEndEvent evt)
    {
        if (evt.target == Root && Root.ClassListContains(_offClass))
            Root.style.display = DisplayStyle.None;
    }

    public virtual void Dispose()
    {
        Root?.UnregisterCallback<TransitionEndEvent>(OnTransitionEnd);
    }
}
