using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
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

        //Trinked set widoww
        _trikedWindow = root.Q<VisualElement>("trinked_window");

        _tReturn = root.Q<Button>("TReturn");


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
        _start.RegisterCallback<ClickEvent>(OnTrinkedButtonClicker);
        _tReturn.RegisterCallback<ClickEvent>(Return);

        //teste de transição
        _trikedWindow.RegisterCallback<TransitionEndEvent>(OnTransicaoFinalizada);

    }


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
    }
        // Update is called once per frame
        void Update()
    {
        
    }
}
