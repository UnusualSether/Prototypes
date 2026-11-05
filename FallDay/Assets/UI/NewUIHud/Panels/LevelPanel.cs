using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>Janela de seleção de nível e início do jogo.</summary>
public class LevelPanel : UIPanel
{
    private readonly Button _return;
    private readonly Button _level1;
    private readonly string _sceneName;

    public LevelPanel(VisualElement root, UISoundManager sound, string sceneName)
        : base(root, UINames.LevelWindow, UINames.LevelOn, UINames.LevelOff)
    {
        _sceneName = sceneName;

        _return = root.Q<Button>(UINames.LevelReturn);
        _level1 = root.Q<Button>(UINames.Level1Button);

        _return?.RegisterCallback<ClickEvent>(OnReturn);
        _level1?.RegisterCallback<ClickEvent>(OnLevel1);

        UIUtils.BindSound(sound, _return, "Confirm");
        UIUtils.BindSound(sound, _level1, "Confirm");
    }

    // Deixa a janela presente por trás da pergunta do tutorial (sem animação de entrada)
    public void ShowBackdrop()
    {
        if (Root != null) Root.style.display = DisplayStyle.Flex;
    }

    private void OnReturn(ClickEvent evt) => Hide();

    // Adicionar suporte a múltiplos níveis futuramente
    private void OnLevel1(ClickEvent evt) => SceneManager.LoadScene(_sceneName);

    public override void Dispose()
    {
        base.Dispose();
        _return?.UnregisterCallback<ClickEvent>(OnReturn);
        _level1?.UnregisterCallback<ClickEvent>(OnLevel1);
    }
}
