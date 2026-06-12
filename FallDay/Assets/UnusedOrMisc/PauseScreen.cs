using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] GameObject PauseMenu;
    Button Continue, Config, Exit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        Continue = uiDocument.rootVisualElement.Q<Button>("Proceed");
        Config = uiDocument.rootVisualElement.Q<Button>("Config");
        Exit = uiDocument.rootVisualElement.Q<Button>("Exit");

        if (Continue != null)
        {
            Continue.clicked += Unpause;
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






    private void ReturnMainMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
