using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        Button startButton = uiDocument.rootVisualElement.Q<Button>("Play");

        if (startButton != null)
        {
            startButton.clicked += SceneLoader;
        }
    }

    private void SceneLoader()
    {
        SceneManager.LoadScene("Scenes/MainScene");
    }

}
