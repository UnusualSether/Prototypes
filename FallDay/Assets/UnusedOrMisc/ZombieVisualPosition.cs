using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

/* public partial class ZombieVisualPosition : MonoBehaviour
{
    private List<Label> warning = new List<Label>();
    private GameHandler handler;
    private Zombie zombie; 

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        //warning = root.Q<Label>("Warning");

        UQueryBuilder<Label> allLabels = root.Query<Label>(className: "warning");
    }

    public Zombie Distance;

    private void Update()
    {
        if(warning != null)
        {
            ZombieModes();
        }
    }

    public void ZombieModes()
    {
        string warningtext = "";

        if ((Input.GetMouseButtonDown(0)))
        //Distance.phase == Zombie.ZombiePhase.Approach
        {
            //text.warning = "!";
            Debug.Log("Working");
        }
        if(Distance.phase == Zombie.ZombiePhase.Close)
        {
            warningtext = "!!";
        }
        else
        {
            warningtext = "";
        }

    }

} */
