using UnityEngine;
using UnityEngine.UIElements;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    private Button PauseButton;

    // Variável para controlar o estado atual do pause
    private bool isPaused = false;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        if (uiDocument != null)
        {
            PauseButton = uiDocument.rootVisualElement.Q<Button>("PauseButton");

            if (PauseButton != null)
            {
                // O botão do UI Toolkit agora é a única forma de pausar/despausar
                PauseButton.clicked += TogglePause;
            }
        }
    }

    private void OnDisable()
    {
        // Boa prática: remove a inscrição do evento quando o objeto for desativado
        if (PauseButton != null)
        {
            PauseButton.clicked -= TogglePause;
        }
    }

    // Função que decide se deve pausar ou despausar
    public void TogglePause()
    {
        if (isPaused)
        {
            Unpause();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0.0000000000001f;
        isPaused = true;
    }

    public void Unpause()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}