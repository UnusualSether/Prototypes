using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public partial class ZombieVisualPosition : MonoBehaviour
{
    public GameObject UIdocument;
    private GameHandler handler;
    private Label[] warning = new Label[4];

    GameDisplay ZombieDisplay;
    Zombie CallZombie;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        //warning = root.Q<Label>("Warning");

        warning[0] = root.Q<Label>("Warning1");  // Não deveria ser [0],[1],[2],[3] envez de [1],[2],[3],[4]?
        warning[1] = root.Q<Label>("Warning2");
        warning[2] = root.Q<Label>("Warning3");
        warning[3] = root.Q<Label>("Warning4");

    }

    /*private void Update()
    {
        if(warning1 != null && warning2 != null && warning3 != null && warning4 != null)
        {
            Visual();
        }
    }*/

    private void Start()
    {
        if (ZombieDisplay != null)
        {
            Visual();
        }
        else
        {
            Debug.Log("Visual Not Working");
        }
    }

    public void Visual()
    {
        int index = CallZombie.id - 1;
        if (index < 0 || index >= warning.Length || warning[index] == null) return;

        Label currentWarning = warning[index];

        if (CallZombie.phase == Zombie.ZombiePhase.Approach)
        {
            currentWarning.text = "!!";
            currentWarning.style.opacity = 1f;
            Debug.Log($"Warning{CallZombie.id} Worked");
        }
        else if (CallZombie.phase == Zombie.ZombiePhase.Close)
        {
            currentWarning.text = "!!!";
            currentWarning.style.opacity = 1f;
        }
    }

}
