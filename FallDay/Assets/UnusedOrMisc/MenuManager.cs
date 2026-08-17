using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    //Set Child Not Parent
    public GameObject Menu;
    public GameObject Config;
    public GameObject Devs;
    public GameObject LevelMenu;
    public GameObject Weapons;
    public GameObject Difficulty;
    public GameObject Trinkets;

    public void OnEnable()
    {

        var uiDocument = GetComponent<UIDocument>();

        if (uiDocument != null)
        {

            Button startbutton = uiDocument.rootVisualElement.Q<Button>("Play");
            Button configbutton = uiDocument.rootVisualElement.Q<Button>("Config");
            Button Devbutton = uiDocument.rootVisualElement.Q<Button>("Credits");
            Button returnbutton = uiDocument.rootVisualElement.Q<Button>("Return");
            Button level1button = uiDocument.rootVisualElement.Q<Button>("level1");
            Button level2button = uiDocument.rootVisualElement.Q<Button>("level2");
            ////////////////////////////////////////////////////////////////////////
            Button begin = uiDocument.rootVisualElement.Q<Button>("StartGame");
            Button returntomenu = uiDocument.rootVisualElement.Q<Button>("ReturnMenu");
            ////////////////////////////////////////////////////////////////////////
            Button diffchoice = uiDocument.rootVisualElement.Q<Button>("ToDifficulty");


            if (startbutton != null)
            {
                startbutton.clicked += trinketsmenu;
            }

            if (diffchoice != null)
            {
                diffchoice.clicked += DiffSelection;
            }

            if (returntomenu != null)
            {
                returntomenu.clicked += ReturnMainMenu;
            }

            if (begin != null)
            {
                begin.clicked += StartGame;
            }

            if (Devbutton != null)
            {
                Devbutton.clicked += DevMenu;
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
                level1button.clicked += StartGame;
            }
            if (level2button != null)
            {
                level2button.clicked += StartGame;
            }
        }
    }

    private void configmenu()
    {
        Config.SetActive(true);
        Menu.SetActive(false);
    }

    private void returnmenu()
    {
        Menu.SetActive(true);
        Config.SetActive(false);
        Devs.SetActive(false);
    }

    private void trinketsmenu()
    {
        Menu.SetActive(false);
        Trinkets.SetActive(true);
    }


    private void DevMenu()
    {
        Devs.SetActive(true);
        Menu.SetActive(false);
    }

    private void ReturnMainMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    private void StartGame()
    {
        SceneManager.LoadScene("PresentableText/PresentableTex");
    }
    private void LevelSelect()
    {
        SceneManager.LoadScene("Scenes/LevelSelect");
    }

    private void Level1()
    {
        //Debug.Log("Level 1");
        //StartGame();
        LevelMenu.SetActive(false);
        Weapons.SetActive(true);
    }
    private void Level2()
    {
        //Debug.Log("Level 2");
        //StartGame();
        LevelMenu.SetActive(false);
        Weapons.SetActive(true);

    }

    private void DiffSelection()
    {
        Weapons.SetActive(false);
        Difficulty.SetActive(true);

    }
}