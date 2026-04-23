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
        ZombieTutorial();
    }

    public void ZombieTutorial()
    {
        text.text = "Aqui é a area aonde os zumbis irão aparecer, quando o cubo ficar verde, você pode clicar nele para mirar no zumbi";
        top.style.height = 100;
        bottom.style.height = 380;
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
        top.style.height = 400;
        left.style.width = 0;
        bottom.style.height = 0;

        next.style.bottom = 410;

        hole.style.height = 450;
        hole.style.width = 450;

        text.text = "Esta é a sua area de ataque, pressione uma peça de uma cor e arraste para outras peças com a mesma cor perto dela";
        text.style.bottom = 160;

        arrow.style.bottom = 10;
        arrow.style.rotate = new Rotate(Angle.Degrees(90));
        arrow.style.left = 180;
    }

    public void HPtutorial()
    {
        top.style.height = 68;
        left.style.width = 66;
        right.style.width = 66;
        bottom.style.height = 1280;

        next.style.bottom = 120;

        hole.style.height = 140;
        hole.style.width = 270;

        text.text = "Esta é a sua barra de HP, toda vez que um zumbi se aproximar de você ele atacara e te causara dano, você perde quando sua vida chegar ao zero";
        text.style.bottom = 100;

        arrow.style.bottom = 120;
        arrow.style.left = 90;
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

