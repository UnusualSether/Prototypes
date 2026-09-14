using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    private Button _closeMenu;
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
        _openlevel.RegisterCallback<ClickEvent>(OnLevelButtonClicker);
        _lReturn.RegisterCallback<ClickEvent>(LevelReturn);

        // Checagem de fim de transição (usada para remover do layout após animação de saída)
        _trikedWindow.RegisterCallback<TransitionEndEvent>(OnTransicaoFinalizada);
        _levelWindow.RegisterCallback<TransitionEndEvent>(OnTransicaoFinalizada);

        // Início do jogo
        _openGame.RegisterCallback<ClickEvent>(StarGame);
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
    private void OnLevelButtonClicker(ClickEvent evt)
    {
        _levelWindow.style.display = DisplayStyle.Flex;

        _levelWindow.schedule.Execute(() =>
        {
            _levelWindow.RemoveFromClassList("level_select_off");
            _levelWindow.AddToClassList("level_select_on");
        });
    }

    // Dispara a animação de saída da janela Level, retornando para a Trinked
    private void LevelReturn(ClickEvent evnt)
    {
        _levelWindow.RemoveFromClassList("level_select_on");
        _levelWindow.AddToClassList("level_select_off");
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
