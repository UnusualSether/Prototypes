using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Vive APENAS no "SystemManager" (objeto persistente, nunca desativado).
/// Versão mínima: só os 3 botões que já existem no projeto.
/// Adicione mais UIDocuments/botões aos poucos, seguindo o mesmo padrão.
/// </summary>
public class MainMenuSystem : MonoBehaviour
{
    [Header("UIDocuments de cada tela (arraste os componentes UIDocument)")]
    [SerializeField] private UIDocument mainMenuDocument;
    [SerializeField] private UIDocument configDocument;
    [SerializeField] private UIDocument trinketDocument;

    [Header("GameObjects das telas (para ativar/desativar)")]
    public GameObject Menu;      // objeto "MainMenu"
    public GameObject Config;    // objeto "Configs"
    public GameObject Trinkets;  // objeto "Trinkets"

    [Header("Nome da cena do mapa do jogo")]
    [SerializeField] private string gameSceneName = "PresentableText/PresentableTex";

    private void Awake()
    {
        // "Return" existe em todos os documentos com o mesmo nome/ID —
        // por isso registramos o mesmo handler em cada um que existir.
        RegisterReturnButton(mainMenuDocument);
        RegisterReturnButton(configDocument);
        RegisterReturnButton(trinketDocument);

        // "Play" só existe na tela do MainMenu
        if (mainMenuDocument != null)
        {
            var playButton = mainMenuDocument.rootVisualElement.Q<Button>("Play");
            playButton?.RegisterCallback<ClickEvent>(OnPlayClicked);
        }
        else
        {
            Debug.LogWarning("MenuManager: mainMenuDocument não atribuído — botão Play não foi registrado.");
        }

        // "ToLevel" — ajuste aqui se o botão estiver em outro documento
        if (trinketDocument != null)
        {
            var toLevelButton = trinketDocument.rootVisualElement.Q<Button>("ToLevel");
            toLevelButton?.RegisterCallback<ClickEvent>(OnToLevelClicked);
        }
    }

    private void RegisterReturnButton(UIDocument document)
    {
        if (document == null) return;

        var returnButton = document.rootVisualElement.Q<Button>("Return");
        returnButton?.RegisterCallback<ClickEvent>(OnReturnClicked);
    }

    private void OnPlayClicked(ClickEvent evt)
    {
        Menu.SetActive(false);
        Trinkets.SetActive(true);
    }

    private void OnReturnClicked(ClickEvent evt)
    {
        Menu.SetActive(true);
        Config.SetActive(false);
        Trinkets.SetActive(false);
    }

    private void OnToLevelClicked(ClickEvent evt)
    {
        // Por enquanto carrega a cena direto.
        // Quando tiver a tela de loading pronta, troque por uma
        // corrotina com SceneManager.LoadSceneAsync + barra de progresso.
        SceneManager.LoadScene(gameSceneName);
    }
}