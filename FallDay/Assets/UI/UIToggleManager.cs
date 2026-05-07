
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIToggleManager : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;
    private Button rtnButton;

    void Start()
    {
        //Referencia UIDOcument
        uiDocument = GetComponent<UIDocument>();

        //Define o elemento raiz PRIMEIRO (Crucial: se fizer a busca antes disso, o jogo quebra)
        root = uiDocument.rootVisualElement;

        // busca o botão pelo nome 
        rtnButton = root.Q<Button>("Return_btn");

        // esconde a uii
        root.style.display = DisplayStyle.None;

        // chama a função atrelada ao botão
        if (rtnButton != null)
        {
            //a função pode ser alterada de acordo com o desejo
            // += chama a função
            rtnButton.clicked += MainMenuReturn;
        }
        else
        {
            // verifcação de erro
            Debug.LogError("Esse btn não esta no layout");
        }
    }

    void Update()
    {
        //test pc
        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleUI();
        }
    }

    private void MainMenuReturn()
    {
        Debug.Log("Botão de Menu inicial pressionado");

        // Time volta a correr antes do retorno ao menu inicial
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

    void OnDisable()
    {
        if (rtnButton != null)
        {
            // -= remove funçaõ da memoria e poupa o cache
            rtnButton.clicked -= MainMenuReturn;
        }
    }

    public void ToggleUI()
    {
        // Mostra a UI
        if (root.style.display == DisplayStyle.None)
        {
            root.style.display = DisplayStyle.Flex;
            Time.timeScale = 0.00000000000000000001f; 
        }
        
        /*else
        {
            root.style.display = DisplayStyle.None;
            Time.timeScale = 1f;
        }*/
    }
}