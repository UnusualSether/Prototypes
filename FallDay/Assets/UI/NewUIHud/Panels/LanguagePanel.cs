using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

/// <summary>Seletor de idioma (setas + nome do idioma) e integração com o Localization.</summary>
public class LanguagePanel
{
    private readonly VisualElement _container;
    private readonly Button _left;
    private readonly Button _right;
    private readonly Label _name;

    public LanguagePanel(VisualElement root, UISoundManager sound)
    {
        _container = root.Q<VisualElement>(UINames.LanguageContainer);
        _left = root.Q<Button>(UINames.LeftLanguage);
        _right = root.Q<Button>(UINames.RightLanguage);
        _name = root.Q<Label>(UINames.LanguageName);

        if (_left != null) _left.clicked += ChangeLanguage;
        if (_right != null) _right.clicked += ChangeLanguage;

        UIUtils.BindSound(sound, _left, "Click");
        UIUtils.BindSound(sound, _right, "Click");

        UpdateText(LocalizationSettings.SelectedLocale);
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    // Mostra (flex) ou esconde (none) o seletor de idioma
    public void SetVisible(bool visible)
    {
        var display = visible ? DisplayStyle.Flex : DisplayStyle.None;

        if (_container != null)
        {
            _container.style.display = display;
            return;
        }

        // Plano B: sem container "Language" no UXML, controla os 3 elementos
        if (_left != null) _left.style.display = display;
        if (_right != null) _right.style.display = display;
        if (_name != null) _name.style.display = display;
    }

    private void ChangeLanguage()
    {
        var languages = LocalizationSettings.AvailableLocales.Locales;
        if (languages.Count == 0) return;

        int currentIndex = languages.IndexOf(LocalizationSettings.SelectedLocale);
        int nextIndex = (currentIndex + 1) % languages.Count;

        LocalizationSettings.SelectedLocale = languages[nextIndex];
    }

    private void OnLocaleChanged(Locale newLocale) => UpdateText(newLocale);

    private void UpdateText(Locale locale)
    {
        if (_name == null || locale == null) return;

        _name.text = locale.Identifier.Code.StartsWith("pt") ? "Português" : "English";
    }

    public void Dispose()
    {
        if (_left != null) _left.clicked -= ChangeLanguage;
        if (_right != null) _right.clicked -= ChangeLanguage;
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }
}
