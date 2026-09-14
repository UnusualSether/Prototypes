using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIGameController : MonoBehaviour
{   //botão de pause em jogo
    private Button _pauseButton;

    //botões do menu pause
    private Button _resumeButton;
    private Button _setting;
    private Button _quit;

    private Button _returnPause;

    //ScreenSet

    private VisualElement _pausePanel;

    private VisualElement _screenButton;
    private VisualElement _screenSetting;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        BindElements(root);
        SetInitialState();
        RegisterCallbacks();


    }


    #region Setup - Busca dos elementos visuais (bind com a UXML)
    private void BindElements(VisualElement root)
    {
        _pauseButton = root.Q<Button>("PauseButton");


        _pausePanel = root.Q<VisualElement>("PausePanel");

        _screenButton = root.Q<VisualElement>("ScreenButton");
        
        _resumeButton = root.Q<Button>("ReturnGame");
        _setting = root.Q<Button>("Setting");
        //sair do jogo
        _quit = root.Q<Button>("Quit");

        //retornar para o pause 
        _screenSetting = root.Q<VisualElement>("ScreenSetting");
        _returnPause = root.Q<Button>("ReturnPause");
        
    }
    #endregion
    #region Setup - Estado inicial da UI
    private void SetInitialState()
    {
        
    }
    #endregion



    #region Setup - Registro de callbacks de clique/transição
    private void RegisterCallbacks()
    {
        //Abrir e fechar pause
        _pauseButton.RegisterCallback<ClickEvent>(OnPauseButtonClicker);
        _resumeButton.RegisterCallback<ClickEvent>(Return);

        //sair do jogo
        _quit.RegisterCallback<ClickEvent>(QuitGame);


        // Checagem de fim de transição
        _pausePanel.RegisterCallback<TransitionEndEvent>(OnTransicaoFinalizada);

    }
    #endregion

    #region Transições - Limpeza pós-animação
    // Após a animação de saída terminar, remove a janela do layout (display: None)
    // para não ocupar espaço/receber interação enquanto estiver invisível
    private void OnTransicaoFinalizada(TransitionEndEvent evt)
    {
        if (_pausePanel.ClassListContains("pause_panel_off"))
        {
            _pausePanel.style.display = DisplayStyle.None;
        }   
    }
    #endregion

    //C

    #region Janela pause
    // Exibe a janela Pause e dispara a classe de animação de entrada
    private void OnPauseButtonClicker(ClickEvent evt)
    {
        _pausePanel.style.display = DisplayStyle.Flex;

        _pausePanel.schedule.Execute(() =>
        {
            _pausePanel.RemoveFromClassList("pause_panel_off");
            _pausePanel.AddToClassList("pause_panel_on");
        });
        Time.timeScale = 0.0000000000001f;
        
    }

    // Dispara a animação de saída da janela pause 
    private void Return(ClickEvent evnt)
    {
        Time.timeScale = 1;
        _pausePanel.RemoveFromClassList("pause_panel_on");
        _pausePanel.AddToClassList("pause_panel_off");
        
    }
    #endregion


    private void QuitGame(ClickEvent evt)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Scenes/MainMenu");
    }

}
