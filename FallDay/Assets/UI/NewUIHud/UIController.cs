using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class UIController : MonoBehaviour
{


    private VisualElement _bottomContainer;

    private Button _openConfig;

    private Button _openShop;

    private VisualElement _bottomSheet;

    private VisualElement _scrim;

    private Button _start;

    private Button _closeMenu;

    //triked
    private VisualElement _trikedWindow;

    private Button _tReturn;

    private Button _openlevel;

    //level
    private VisualElement _levelWindow;

    private Button _lReturn;
    //setlevelstart

    private Button _openGame;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        //hubbar
        _bottomContainer = root.Q<VisualElement>("Container_Bottom");
        //botão do menu
        _openConfig = root.Q<Button>("openConfig");
        //botão do shop temporario depois colocar 1 botão para fechar
        _openShop = root.Q<Button>("openShop");

        _bottomSheet = root.Q<VisualElement>("BottomSheet");
        _scrim = root.Q<VisualElement>("Scrim");

        _closeMenu = root.Q<Button>("closeMenu");

        /*Start set*/

        _start = root.Q<Button>("Play");

        //Trinked set window
        _trikedWindow = root.Q<VisualElement>("trinked_window");

        _tReturn = root.Q<Button>("TReturn");

        //Level set window
        _openlevel = root.Q<Button>("Difficulty_btn");

        _levelWindow = root.Q<VisualElement>("level_window");

        _lReturn = root.Q<Button>("return_to_trinked");

        //starGamelevel add psoteriro mente metodo de multilevel

        _openGame = root.Q<Button>("level1");

        ///
        /////////////
        ///

        //set botão off

        _scrim.style.display = DisplayStyle.None;


        ///
        //////////////////
        ///
        //configuração de botão
        _openConfig.RegisterCallback<ClickEvent>(OnOpenButtonClicker);
        _closeMenu.RegisterCallback<ClickEvent>(OnCloseButtonClicker);
        //config trinked window
        _start.RegisterCallback<ClickEvent>(OnTrinkedButtonClicker);
        _tReturn.RegisterCallback<ClickEvent>(Return);
        //config level window
        _openlevel.RegisterCallback<ClickEvent>(OnLevelButtonClicker);
        _lReturn.RegisterCallback<ClickEvent>(LevelReturn);


        //return e open trinked transição check
        _trikedWindow.RegisterCallback<TransitionEndEvent>(OnTransicaoFinalizada);
        //return e open level transição check
        _levelWindow.RegisterCallback<TransitionEndEvent>(OnTransicaoFinalizada);



        ///
        //Game Start
        _openGame.RegisterCallback<ClickEvent>(StarGame);

    }

    //return e open trinked

    private void Return(ClickEvent evnt)
    {
        _trikedWindow.RemoveFromClassList("trinked_select_on");
        _trikedWindow.AddToClassList("trinked_select_off");
    }

    /*Set trinked window*/

    private void OnTrinkedButtonClicker(ClickEvent evt)
    {
        _trikedWindow.style.display = DisplayStyle.Flex;


        //teste
        _trikedWindow.schedule.Execute(() =>
        {

            _trikedWindow.RemoveFromClassList("trinked_select_off");
            _trikedWindow.AddToClassList("trinked_select_on");
        });

    }
    //


    //return e open level

    private void LevelReturn(ClickEvent evnt)
    {
        _levelWindow.RemoveFromClassList("level_select_on");
        _levelWindow.AddToClassList("level_select_off");
    }


    /*Set level window*/

    private void OnLevelButtonClicker(ClickEvent evt)
    {
        _levelWindow.style.display = DisplayStyle.Flex;


        //teste
        _levelWindow.schedule.Execute(() =>
        {

            _levelWindow.RemoveFromClassList("level_select_off");
            _levelWindow.AddToClassList("level_select_on");
        });

    }


    private void OnOpenButtonClicker(ClickEvent evt)
    {
        
        _scrim.style.display = DisplayStyle.Flex;
        
        _bottomSheet.AddToClassList("bottom_Sheet--UP");
        _scrim.AddToClassList("scrim_fadein");
        _start.AddToClassList("starOff");
        _closeMenu.AddToClassList("close_menu_on");

    }
    private void OnCloseButtonClicker(ClickEvent evt)
    {
        
        _scrim.style.display = DisplayStyle.None;
        
        _bottomSheet.RemoveFromClassList("bottom_Sheet--UP");
        _scrim.RemoveFromClassList("scrim_fadein");
        _start.RemoveFromClassList("starOff");
        _closeMenu.RemoveFromClassList("close_menu_on");
    }



    private void OnTransicaoFinalizada(TransitionEndEvent evt)
    {
        // Se a animação terminou e o elemento está com a classe de oculto, remove do layout
        if (_trikedWindow.ClassListContains("trinked_select_off"))
        {
            _trikedWindow.style.display = DisplayStyle.None;
        }

        if (_levelWindow.ClassListContains("level_select_off"))
        {
            _levelWindow.style.display = DisplayStyle.None;
        }



    }

    private void StarGame(ClickEvent evt)
    {
        SceneManager.LoadScene("PresentableText/PresentableTex");
    }
        // Update is called once per frame
        void Update()
    {
        
    }
}
