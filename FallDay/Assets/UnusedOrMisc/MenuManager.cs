using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        Button startbutton = uiDocument.rootVisualElement.Q<Button>("Play");

        Button configbutton = uiDocument.rootVisualElement.Q<Button>("Config");

        Button returnbutton = uiDocument.rootVisualElement.Q<Button>("Return");

        if (startbutton != null)
        {
            startbutton.clicked += StartGame;
        }

        if (configbutton != null)
        {
            configbutton.clicked += configmenu;
        }

        if (returnbutton != null)
        {
            returnbutton.clicked += returnmenu;
        }
    }

    private void configmenu()
    {
        SceneManager.LoadScene("Scenes/Configs");
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Scenes/MainScene");
    }

    private void returnmenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
