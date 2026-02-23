using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UIElements;

public class MenuFunctionality : MonoBehaviour
{
    public VisualElement ui;

    public GachaSystem system;

    public Button pull;
    public Label fragmentDisplay;


    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }
    private void OnEnable()
    {
        pull = ui.Q<Button>("PullButton");

        fragmentDisplay = ui.Q<Label>("FragmentCount");

        pull.clicked += system.Pull;
    }

    private void Update()
    {
        ChangeFragDisplay();
    }

    private void ChangeFragDisplay()
    {
        
    }
}
