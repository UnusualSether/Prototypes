using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Único MonoBehaviour da interface do menu principal.
/// Cria os painéis, liga um ao outro e descarta tudo ao desativar.
/// Cada painel cuida da sua própria parte (ver pasta Panels).
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class UIController : MonoBehaviour
{
    [Header("Áudio")]
    [SerializeField] private UISoundManager _sound;

    [Header("Bottom Sheet")]
    [SerializeField] private long sheetTransitionMs = 350; // igual à duração da transition no USS

    [Header("Jogo")]
    [SerializeField] private string firstLevelScene = "Level1";

    private LanguagePanel _language;
    private BottomSheetPanel _bottomSheet;
    private LevelPanel _level;
    private TutorialPanel _tutorial;
    private TrinkedPanel _trinked;

    private bool _built;

    void OnEnable() => TryBuild();

    // Garantia: se o UIDocument ainda não tinha criado a árvore no OnEnable, monta aqui
    void Start() => TryBuild();

    void OnDisable() => Teardown();

    private void TryBuild()
    {
        if (_built) return;

        var root = GetComponent<UIDocument>().rootVisualElement;
        if (root == null || root.childCount == 0) return;

        _language = new LanguagePanel(root, _sound);
        _bottomSheet = new BottomSheetPanel(root, _sound, _language, sheetTransitionMs);
        _level = new LevelPanel(root, _sound, firstLevelScene);
        _tutorial = new TutorialPanel(root, _sound, onFinished: _level.Show);

        // Trinked -> botão de dificuldade: mostra o fundo do Level e abre a pergunta do tutorial
        _trinked = new TrinkedPanel(root, _sound, onOpenLevel: () =>
        {
            _level.ShowBackdrop();
            _tutorial.ShowQuestion();
        });

        UIUtils.Pulse(root.Q<Image>(UINames.TapToStart), min: 0.9f, max: 1.1f, speed: 2f);

        _built = true;
    }

    private void Teardown()
    {
        _language?.Dispose();
        _bottomSheet?.Dispose();
        _level?.Dispose();
        _tutorial?.Dispose();
        _trinked?.Dispose();

        _language = null;
        _bottomSheet = null;
        _level = null;
        _tutorial = null;
        _trinked = null;
        _built = false;
    }
}
