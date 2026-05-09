using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public partial class TutorialControler : MonoBehaviour
{
    private VisualElement top, left, right, hole, bottom, arrow;
    private Label text;
    private Button next;

    public GameObject Tutorial;

    private int tutorialpage = 1;

    public static bool TutorialEnded = false;

    private void Start()
    {
        Tutorial.SetActive(true);
        ZombieTutorial();
    }

    public void ZombieTutorial()
    {
        text.text = "Aqui é a area aonde os zumbis irão aparecer, quando o cubo ficar verde, você pode clicar nele para mirar no zumbi";
        top.style.height = new Length(50, LengthUnit.Percent);
        bottom.style.height = new Length(66, LengthUnit.Percent);
        arrow.style.opacity = 100;
        next.style.opacity = 100;
    }

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        top = root.Q<VisualElement>("Top");
        left = root.Q<VisualElement>("Left");
        right = root.Q<VisualElement>("Right");
        bottom = root.Q<VisualElement>("Bottom");
        arrow = root.Q<VisualElement>("Arrow");
        hole = root.Q<VisualElement>("Hole");
        text = root.Q<Label>("Text");
        next = root.Q<Button>("Next");

        next.clicked += NextPart;
    }

    void NextPart()
    {
        tutorialpage++;

        if (tutorialpage == 2)
        {
            Match3Tutorial();
        }

        if (tutorialpage == 3)
        {
            HPtutorial();
        }

        if (tutorialpage == 4)
        {
            EndTutorial();
        }
    }

    public void Match3Tutorial()
    {
        top.style.height = new Length(55, LengthUnit.Percent);
        left.style.width = 0;
        bottom.style.height = new Length(0, LengthUnit.Percent);

        next.style.bottom = 100;

        hole.style.height = 450;
        hole.style.width = 450;

        text.text = "Esta é a sua area de ataque, pressione uma peça de uma cor e arraste para outras peças com a mesma cor perto dela";
        text.style.top = 192;

        arrow.style.top = 556;
        arrow.style.rotate = new Rotate(Angle.Degrees(90));
        arrow.style.left = 628;
    }

    public void HPtutorial()
    {
        top.style.height = new Length(52, LengthUnit.Percent);
        left.style.width = new Length(70, LengthUnit.Percent);
        left.style.height = new Length(132, LengthUnit.Percent);
        right.style.width = new Length(75, LengthUnit.Percent);
        right.style.height = new Length(132, LengthUnit.Percent);
        bottom.style.height = new Length(87, LengthUnit.Percent);

        next.style.bottom = -722;

        hole.style.height = new Length(90, LengthUnit.Percent);
        hole.style.width = new Length(270, LengthUnit.Percent);

        text.text = "Esta é a sua barra de HP, toda vez que um zumbi se aproximar de você ele atacara e te causara dano, você perde quando sua vida chegar ao zero";
        text.style.top = 350;

        arrow.style.top = 10;
        arrow.style.rotate = new Rotate(Angle.Degrees(270));
        arrow.style.left = 300;
    }

    private void EndTutorial()
    {
       if (Tutorial != null)
       {
        Tutorial.SetActive(false);
            TutorialEnded = true;
        }
    }

}

