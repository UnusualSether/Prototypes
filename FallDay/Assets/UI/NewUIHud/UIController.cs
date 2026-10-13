using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

/// <summary>
/// Controla toda a navegação de UI do menu principal:
/// - Hub inferior (abrir/fechar menu de configurações)
/// - Janela "Trinked" (seleção inicial pós start)
/// - Janela "Level" (seleção de dificuldade/fase)
/// - Início do jogo (troca de cena)
/// </summary>
public class UIController : MonoBehaviour
{
    #region Referências - Hub Inferior (barra fixa com botões de acesso)
    // Container da barra inferior e botões que abrem o menu (config) e o shop
    private VisualElement _bottomContainer;
    private Button _openConfig;
    private Button _openShop;
    #endregion

    #region Referências - Bottom Sheet / Menu de Configurações
    // Painel deslizante (bottom sheet) que sobe ao clicar em "config",
    // junto do fundo escurecido (scrim) e do botão de fechar
    private VisualElement _bottomSheet;
    private VisualElement _scrim;
    private Button _closeMenu, LeftLang, RightLang;
    private Label LangText;
    #endregion

    #region Referências - Botão Start
    // Botão principal que abre a janela "Trinked" (primeira etapa antes do jogo)
    private Button _start;
    #endregion

    #region Referências - Janela Trinked
    // Janela intermediária exibida após o Start; permite avançar para a seleção de nível
    private VisualElement _trikedWindow;
    private Button _tReturn;      // volta/fecha a janela Trinked
    private Button _openlevel;    // avança para a janela de seleção de nível
    #endregion

    #region Referências - Janela Level (Seleção de Dificuldade/Fase)
    // Janela onde o jogador escolhe o nível/dificuldade
    private VisualElement _levelWindow;
    private Button _lReturn;      // volta da janela Level para a janela Trinked
    #endregion

    #region Referências - Janeça Tutorial
    //janela aonde o jogador
    private VisualElement _tutorialWindow;
    private Button _tutorialy;
    private Button _tutorialn;

    #endregion
    private VisualElement _tutorialpages;
    private List<VisualElement> _tutorialPages = new List<VisualElement>();
    private int _currentTutorialPage = 0;
    private Button _nextpage;

    #region Referências - Início do Jogo
    // Botão que efetivamente carrega a cena do jogo (nível 1)
    private Button _openGame;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        // Hub inferior
        _bottomContainer = root.Q<VisualElement>("Container_Bottom");
        _openConfig = root.Q<Button>("openConfig");
        _openShop = root.Q<Button>("openShop"); // botão temporário do shop; futuramente unificar com botão de fechar

        // Bottom sheet / menu de configurações
        _bottomSheet = root.Q<VisualElement>("BottomSheet");
        _scrim = root.Q<VisualElement>("Scrim");
        _closeMenu = root.Q<Button>("closeMenu");

        // Start
        _start = root.Q<Button>("Play");

        // Janela Trinked
        _trikedWindow = root.Q<VisualElement>("trinked_window");
        _tReturn = root.Q<Button>("TReturn");
        _openlevel = root.Q<Button>("Difficulty_btn");

        // Janela Level
        _levelWindow = root.Q<VisualElement>("level_window");
        _lReturn = root.Q<Button>("return_to_trinked");

        //Janela Tutorial

        _tutorialWindow = root.Q<VisualElement>("tutorial_window");
        _tutorialpages = root.Q<VisualElement>("TutorialWindows");
        _nextpage = root.Q<Button>("NextPage");


        _tutorialPages.Clear();
        _tutorialPages.Add(root.Q<VisualElement>("TutorialWindow1"));
        _tutorialPages.Add(root.Q<VisualElement>("TutorialWindow2"));
        _tutorialPages.Add(root.Q<VisualElement>("TutorialWindow3"));
        _tutorialPages.Add(root.Q<VisualElement>("TutorialWindow4"));
        _tutorialPages.Add(root.Q<VisualElement>("TutorialWindow5"));
        _tutorialPages.Add(root.Q<VisualElement>("TutorialWindow6"));

        _tutorialy = root.Q<Button>("TutorialYes");
        _tutorialn = root.Q<Button>("TutorialNo");


        // Início do jogo (adicionar suporte a múltiplos níveis futuramente)
        _openGame = root.Q<Button>("level1");
    }
    #endregion

    #region Setup - Estado inicial da UI
    private void SetInitialState()
    {
        // Scrim (fundo escurecido do bottom sheet) começa oculto
        _scrim.style.display = DisplayStyle.None;
    }
    #endregion

    #region Setup - Registro de callbacks de clique/transição
    private void RegisterCallbacks()
    {
        // Hub inferior / bottom sheet
        _openConfig.RegisterCallback<ClickEvent>(OnOpenButtonClicker);
        _closeMenu.RegisterCallback<ClickEvent>(OnCloseButtonClicker);

        // Janela Trinked
        _start.RegisterCallback<ClickEvent>(OnTrinkedButtonClicker);
        _tReturn.RegisterCallback<ClickEvent>(Return);

        // Janela Level
        _openlevel.RegisterCallback<ClickEvent>(OnTutorial /*OnLevelButtonClicker*/);
        _lReturn.RegisterCallback<ClickEvent>(LevelReturn);

        //Janela Tutorial

        _tutorialWindow.RegisterCallback<TransitionEndEvent>(OnTransicaoFinalizada);

        if (_nextpage != null)
        {
            _nextpage.RegisterCallback<ClickEvent>(NextTutorial);
        }

        if(_tutorialy != null)
        {
            _tutorialy.clicked += TutorialAnswer;
        }

        if (_tutorialn != null)
        {
            _tutorialn.clicked += TutorialAnswerNo;
        }

        // Checagem de fim de transição (usada para remover do layout após animação de saída)
        _trikedWindow.RegisterCallback<TransitionEndEvent>(OnTransicaoFinalizada);
        _levelWindow.RegisterCallback<TransitionEndEvent>(OnTransicaoFinalizada);

        // Início do jogo
        _openGame.RegisterCallback<ClickEvent>(StarGame);
    }
    #endregion

    #region Setup - Sistema de Tradução
    //set animation tap to start
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        if (root != null)
        {
            LeftLang = root.Q<Button>("LeftLanguage");
            RightLang = root.Q<Button>("RightLanguage");
            LangText = root.Q<Label>("LanguageName");

            if (LeftLang != null)
            {
                LeftLang.clicked += LanguageChange;
            }

            if (RightLang != null)
            {
                RightLang.clicked += LanguageChange;
            }
        }

        LanguageText(LocalizationSettings.SelectedLocale);
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;


        Pulse(root.Q<Image>("TapToStart"), min: 0.9f, max: 1.1f, speed: 2f);

    }

    private void OnDisable()
    {
        if (LeftLang != null)
        {
            LeftLang.clicked -= LanguageChange;
        }
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

        if (RightLang != null)
        {
            RightLang.clicked -= LanguageChange;
        }
    }

    void OnLocaleChanged(Locale newLocale)
    {
        LanguageText(newLocale);
    }

    void LanguageLocaleChange()
    {
        if (LeftLang != null)
        {
            LeftLang.clicked += LanguageChange;
        }

        if (RightLang != null)
        {
            RightLang.clicked += LanguageChange;
        }
    }


    private void Pulse(VisualElement el, float min, float max, float speed)
    {
        if (el == null) return;

        el.schedule.Execute(() =>
        {
            float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
            float s = Mathf.Lerp(min, max, t);
            el.style.scale = new Scale(new Vector3(s, s, 1f));
        }).Every(16);
    }

    void LanguageChange()
    {
        var Languages = LocalizationSettings.AvailableLocales.Locales;
        if (Languages.Count == 0) return;

        int currentIndex = Languages.IndexOf(LocalizationSettings.SelectedLocale);

        int nextIndex = (currentIndex + 1) % Languages.Count;

        LocalizationSettings.SelectedLocale = Languages[nextIndex];
    }

    void LanguageText(Locale CurrentLanguage)
    {
        if (LangText != null && CurrentLanguage != null)
        {
            string text = CurrentLanguage.Identifier.Code;

            if (text.StartsWith("pt"))
            {
                LangText.text = "Português";
            }
            else
            {
                LangText.text = "English";
            }

        }
    }

    #endregion

    #region Bottom Sheet / Menu de Configurações
    // Abre o menu: exibe o scrim e anima a entrada do bottom sheet
    private void OnOpenButtonClicker(ClickEvent evt)
    {
        _scrim.style.display = DisplayStyle.Flex;

        _bottomSheet.AddToClassList("bottom_Sheet--UP");
        _scrim.AddToClassList("scrim_fadein");
        _start.AddToClassList("starOff");
        _closeMenu.AddToClassList("close_menu_on");
    }

    // Fecha o menu: oculta o scrim e reverte as classes de animação
    private void OnCloseButtonClicker(ClickEvent evt)
    {
        _scrim.style.display = DisplayStyle.None;

        _bottomSheet.RemoveFromClassList("bottom_Sheet--UP");
        _scrim.RemoveFromClassList("scrim_fadein");
        _start.RemoveFromClassList("starOff");
        _closeMenu.RemoveFromClassList("close_menu_on");
    }
    #endregion

    #region Janela Trinked (etapa inicial pós Start)
    // Exibe a janela Trinked e dispara a classe de animação de entrada
    private void OnTrinkedButtonClicker(ClickEvent evt)
    {
        _trikedWindow.style.display = DisplayStyle.Flex;

        _trikedWindow.schedule.Execute(() =>
        {
            _trikedWindow.RemoveFromClassList("trinked_select_off");
            _trikedWindow.AddToClassList("trinked_select_on");
        });
    }

    // Dispara a animação de saída da janela Trinked (o display é removido em OnTransicaoFinalizada)
    private void Return(ClickEvent evnt)
    {
        _trikedWindow.RemoveFromClassList("trinked_select_on");
        _trikedWindow.AddToClassList("trinked_select_off");
    }
    #endregion

    #region Janela Level (seleção de dificuldade/fase)
    // Exibe a janela Level e dispara a classe de animação de entrada
    private void OnLevelButtonClicker()
    {
        if (_levelWindow != null)
        {
            _levelWindow.style.display = DisplayStyle.Flex;
            _levelWindow.RemoveFromClassList("level_select_off");

            _levelWindow.schedule.Execute(() =>
            {
                _levelWindow.AddToClassList("level_select_on");
            });
        }
    }


    // Dispara a animação de saída da janela Level, retornando para a Trinked
    private void LevelReturn(ClickEvent evnt)
    {
        _levelWindow.RemoveFromClassList("level_select_on");
        _levelWindow.AddToClassList("level_select_off");
    }
    #endregion

    #region Janela Tutorial

    private void OnTutorial(ClickEvent evt)
    {
        _levelWindow.style.display = DisplayStyle.Flex;
        _tutorialWindow.style.display = DisplayStyle.Flex;

        _tutorialWindow.RemoveFromClassList("Tutorial-Menu_off");
        _tutorialWindow.schedule.Execute(() =>
        {
            _tutorialWindow.AddToClassList("tutorial-Menu_on");
        });


    }

    private void TutorialAnswer()
    {

        CloseTutorialQuestion();

        if(_tutorialpages != null)
        {
            _tutorialpages.style.display = DisplayStyle.Flex;
            _currentTutorialPage = 0;
            CurrentPage(_currentTutorialPage);

            _tutorialpages.RemoveFromClassList("Tutorial-Menu_off");
            _tutorialpages.schedule.Execute(() =>
            {
                _tutorialpages.AddToClassList("Tutorial-Menu_on");
            });
        }  
    }


    private void TutorialAnswerNo()
    {

        CloseTutorialQuestion();

        OnLevelButtonClicker();
    }

    private void CloseTutorialQuestion()
    {
        if (_tutorialWindow != null)
        {
            _tutorialWindow.style.display = DisplayStyle.None;
            _tutorialWindow.RemoveFromClassList("Tutorial-Menu_on");
            _tutorialWindow.AddToClassList("Tutorial-Menu_off");
        }
    }

    private void NextTutorial(ClickEvent evt)
    {
        evt.StopPropagation();

        _currentTutorialPage++;
        
        if(_currentTutorialPage >= _tutorialPages.Count)
        {
            EndTutorial();
        }
        else
        {
            CurrentPage(_currentTutorialPage);
        }

    }

    private void CurrentPage(int pagenumber)
    {
        for (int i = 0; i < _tutorialPages.Count; i++)
        {
            if (_tutorialPages[i] != null)
            {
                _tutorialPages[i].style.display = (i == pagenumber) ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }

    private void EndTutorial()
    {
        if (_tutorialpages != null)
        {
            _tutorialpages.style.display = DisplayStyle.None;
            _tutorialpages.RemoveFromClassList("Tutorial-Menu_on");
            _tutorialpages.AddToClassList("Tutorial-Menu_off");
        }

        if (_tutorialWindow != null)
        {
            _tutorialWindow.style.display = DisplayStyle.None;
            _tutorialWindow.RemoveFromClassList("Tutorial-Menu_on");
            _tutorialWindow.AddToClassList("Tutorial-Menu_off");
        }
        OnLevelButtonClicker();
    }

    #endregion

    #region Transições - Limpeza pós-animação
    // Após a animação de saída terminar, remove a janela do layout (display: None)
    // para não ocupar espaço/receber interação enquanto estiver invisível
    private void OnTransicaoFinalizada(TransitionEndEvent evt)
    {
        if (_trikedWindow.ClassListContains("trinked_select_off"))
        {
            _trikedWindow.style.display = DisplayStyle.None;
        }

        if (_levelWindow.ClassListContains("level_select_off"))
        {
            _levelWindow.style.display = DisplayStyle.None;
        }

        if (evt.target == _tutorialpages && _tutorialpages.ClassListContains("Tutorial-Menu_off"))
        {
            _tutorialpages.style.display = DisplayStyle.None;
        }
    }
    #endregion

    #region Início do Jogo
    // Carrega a cena do jogo ao selecionar o nível
    private void StarGame(ClickEvent evt)
    {
        SceneManager.LoadScene("PresentableText/PresentableTex");
    }
    #endregion

    // Update is called once per frame
    void Update()
    {

    }
}
