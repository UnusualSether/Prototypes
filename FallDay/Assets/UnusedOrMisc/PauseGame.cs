using UnityEngine;
using UnityEngine.UIElements;

public class PauseGame : MonoBehaviour
{
    [SerializeField]GameObject PauseMenu;
    Button PauseButton;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        PauseButton = uiDocument.rootVisualElement.Q<Button>("PauseButton");

        if (PauseButton != null)
        {
            PauseButton.clicked += Pause;
        }

    }

    public void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Unpause()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
