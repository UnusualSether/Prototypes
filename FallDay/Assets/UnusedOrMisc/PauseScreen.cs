using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] GameObject PauseMenu, ConfigMM;
    Button Continue, Config, Exit, Return;

    public void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        Continue = uiDocument.rootVisualElement.Q<Button>("Proceed");
        Config = uiDocument.rootVisualElement.Q<Button>("Config");
        Exit = uiDocument.rootVisualElement.Q<Button>("Exit");
        Return = uiDocument.rootVisualElement.Q<Button>("Return");

        if (Continue != null)
        {
            Continue.clicked += Unpause;
        }

        if (Config != null)
        {
            Config.clicked += Configs;
        }

        if (Return != null)
        {
            Return.clicked += PauseMenuReturn;
        }

        if (Exit != null)
        {
            Exit.clicked += ReturnMainMenu;
        }

    }

    private void Unpause()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    private void Configs()
    {
        ConfigMM.SetActive(true);
    }

    private void PauseMenuReturn()
    {
        ConfigMM.SetActive(false);
    }

    private void ReturnMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
