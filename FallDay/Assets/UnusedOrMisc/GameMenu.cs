using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuMenu : MonoBehaviour
{

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        Button returnbutton = uiDocument.rootVisualElement.Q<Button>("Return");

        if (returnbutton != null)
        {
            returnbutton.clicked += returnmenu;
        }
    }

    private void returnmenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
