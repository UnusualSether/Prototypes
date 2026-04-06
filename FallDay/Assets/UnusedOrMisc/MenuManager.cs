using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        //Button startButton = uiDocument.rootVisualElement.Q<Button>("Play");

        if (uiDocument != null)
        {
            //startButton.clicked += SceneLoader;

            Button startbutton = uiDocument.rootVisualElement.Q<Button>("Play");
            Button configbutton = uiDocument.rootVisualElement.Q<Button>("Config");
            Button returnbutton = uiDocument.rootVisualElement.Q<Button>("Return");
            Button level1button = uiDocument.rootVisualElement.Q<Button>("level1");
            Button level2button = uiDocument.rootVisualElement.Q<Button>("level2");

            if (startbutton != null)
            {
                startbutton.clicked += LevelSelect;
            }

            if (configbutton != null)
            {
                configbutton.clicked += configmenu;
            }

            if (returnbutton != null)
            {
                returnbutton.clicked += returnmenu;
            }

            if (level1button != null)
            {
                level1button.clicked += Level1;
            }
            if (level2button != null)
            {
                level2button.clicked += Level2;
            }
        }
    }
        

    private void SceneLoader()
    {
        SceneManager.LoadScene("Scenes/MainScene");
    }

    private void configmenu()
    {
        SceneManager.LoadScene("Scenes/Configs");
    }
    private void StartGame()
    {
        SceneManager.LoadScene("PresentableText/PresentableTex");
    }
    private void LevelSelect()
    {
        SceneManager.LoadScene("Scenes/LevelSelect");
    }
    private void returnmenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
    private void Level1()
    {
        //Debug.Log("Level 1");
        StartGame();
    }
    private void Level2()
    {
        //Debug.Log("Level 2");
        StartGame();
    }
}