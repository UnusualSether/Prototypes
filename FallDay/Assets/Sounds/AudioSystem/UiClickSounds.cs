using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Sons de UI automáticos para todo um UIDocument (menu, pausa, HUD...), sem ligar botão por botão.
/// Coloque um componente em CADA GameObject que tenha um UIDocument.
///
/// - Clique: todo Button/Toggle e todo cabeçalho de aba do TabView toca "clickId".
///   Outros elementos podem receber a classe USS "click-sound".
/// - Som específico: a classe USS "sfx-NOME" faz o elemento tocar o som NOME no lugar do clique padrão
///   (ex.: classe "sfx-Confirm" toca o id "Confirm").
/// - Slider: todo Slider toca "slideId" ao ser movido (o cooldown do SfxEntry evita spam). Vazio = desligado.
/// - A classe USS "no-click-sound" desativa qualquer som em um elemento (e nos filhos dele).
/// - Debug Log: mostra no Console em qual elemento você clicou e o que foi decidido.
/// </summary>
public class UiClickSounds : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private string clickId = "ui_click";
    [SerializeField] private string slideId = "ui_slide";
    [SerializeField] private bool debugLog = false;

    private const string SfxClassPrefix = "sfx-";
    private const string TabHeaderClass = "unity-tab__header";

    private VisualElement root;
    private float ignoreUntil;

    private void OnEnable()
    {
        root = document.rootVisualElement;

        // Ignora eventos de slider do início (valores iniciais sendo aplicados pelos scripts)
        ignoreUntil = Time.unscaledTime + 0.5f;

        root.RegisterCallback<ClickEvent>(OnClick, TrickleDown.TrickleDown);
        root.RegisterCallback<ChangeEvent<float>>(OnSlide, TrickleDown.TrickleDown);
    }

    private void OnDisable()
    {
        if (root == null) return;
        root.UnregisterCallback<ClickEvent>(OnClick, TrickleDown.TrickleDown);
        root.UnregisterCallback<ChangeEvent<float>>(OnSlide, TrickleDown.TrickleDown);
    }

    private void OnClick(ClickEvent evt)
    {
        var target = evt.target as VisualElement;

        // Sobe a hierarquia: o clique pode cair em um ícone/label dentro do botão
        for (var ve = target; ve != null && ve != root; ve = ve.parent)
        {
            if (ve.ClassListContains("no-click-sound"))
            {
                if (debugLog) Debug.Log($"[UiClickSounds] '{target.name}' bloqueado por no-click-sound (em '{ve.name}').", this);
                return;
            }

            string custom = FindCustomId(ve);
            if (custom != null)
            {
                Fire(custom, target);
                return;
            }

            if (ve is Button || ve is Toggle || ve.ClassListContains("click-sound") || ve.ClassListContains(TabHeaderClass))
            {
                Fire(clickId, target);
                return;
            }
        }

        if (debugLog)
            Debug.Log($"[UiClickSounds] clique em '{target?.name}' sem som: não é botão, toggle, aba, nem tem a classe click-sound.", this);
    }

    private void OnSlide(ChangeEvent<float> evt)
    {
        if (string.IsNullOrEmpty(slideId) || Time.unscaledTime < ignoreUntil) return;
        if (!(evt.target is Slider)) return;

        for (var ve = evt.target as VisualElement; ve != null && ve != root; ve = ve.parent)
        {
            // O ScrollView tem um Slider interno: não toca som ao rolar a lista
            if (ve is Scroller || ve.ClassListContains("no-click-sound")) return;
        }

        AudioManager.Play(slideId);
    }

    private void Fire(string id, VisualElement target)
    {
        if (debugLog) Debug.Log($"[UiClickSounds] clique em '{target?.name}' -> toca '{id}'.", this);
        AudioManager.Play(id);
    }

    private static string FindCustomId(VisualElement ve)
    {
        foreach (var cls in ve.GetClasses())
        {
            if (cls.Length > SfxClassPrefix.Length && cls.StartsWith(SfxClassPrefix))
                return cls.Substring(SfxClassPrefix.Length);
        }
        return null;
    }
}