using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class DevInfo : MonoBehaviour
{
    [SerializeField] private AudioClip PaperSound;
    [SerializeField] private AudioSource Source;
    private Button LF, LG, LS, PM, Papers;
    private Label Info;
    private VisualElement Allinfo;
    private List<Button> ButtonsFlash;

    private float Flash = 3f;
    private float MinOpacity = 0.2f;
    private float MaxOpacity = 1.0f;

    public GameObject Dev;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
       private void OnEnable()
       {
        var uiDocument = GetComponent<UIDocument>();


        LF = uiDocument.rootVisualElement.Q<Button>("LF");
        LG = uiDocument.rootVisualElement.Q<Button>("LG");
        LS = uiDocument.rootVisualElement.Q<Button>("LS");
        PM = uiDocument.rootVisualElement.Q<Button>("PM");
        Papers = uiDocument.rootVisualElement.Q<Button>("ReturnPapers");
        Info = uiDocument.rootVisualElement.Q<Label>("Information");
        Allinfo = uiDocument.rootVisualElement.Q<VisualElement>("info");

        ButtonsFlash = new List<Button> { LF, LS, LG, PM };


        if (LF != null)
        {
            LF.clicked += LFinfo;
        }

        if (LG != null)
        {
            LG.clicked += LGinfo;
        }

        if (LS != null)
        {
            LS.clicked += LSinfo;
        }

        if (PM != null)
        {
            PM.clicked += PMinfo;
        }

        if (Papers != null)
        {
            Papers.clicked += ReturnPaper;
        }


       }

    public void Start()
    {
        Allinfo.style.display = DisplayStyle.None;
    }

    private void PlayPaperSound()
    {
        if(PaperSound != null && Source != null)
        {
            Source.PlayOneShot(PaperSound);
        }
    }

    public void LFinfo()
    {
        Debug.Log("Apertado");
        if(Info != null)
        {
            Info.text = "Lucas Feitosa\n\nProgramador";
            PlayPaperSound();
        }

        if(Allinfo != null)
        {
            Allinfo.style.display = DisplayStyle.Flex;
        }

    }

    public void LSinfo()
    {
        Debug.Log("Apertado");
        if (Info != null)
        {
            Info.text = "Lucas Scott\n\nProgramador";
            PlayPaperSound();
        }

        if (Allinfo != null)
        {
            Allinfo.style.display = DisplayStyle.Flex;
        }

    }

    public void LGinfo()
    {
        Debug.Log("Apertado");
        if (Info != null)
        {
            Info.text = "Luiz Gustavo\n\nProgramador";
            PlayPaperSound();
        }

        if (Allinfo != null)
        {
            Allinfo.style.display = DisplayStyle.Flex;
        }

    }

    public void PMinfo()
    {
        Debug.Log("Apertado");
        if (Info != null)
        {
            Info.text = "Pedro Marcondes\n\nArtista 2D\nModelador 3D";
            PlayPaperSound();
        }

        if (Allinfo != null)
        {
            Allinfo.style.display = DisplayStyle.Flex;
        }

    }

    private void Update()
    {
        if (ButtonsFlash == null) return;

        float rawSin = Mathf.Sin(Time.time * Flash);
        float normalizedSin = (rawSin + 1f) / 2f;
        float Opacity = Mathf.Lerp(MinOpacity, MaxOpacity, normalizedSin);

        foreach (Button btn in ButtonsFlash)
        {
            if(btn != null)
            {
                btn.style.opacity = Opacity;
            }
        }
    }



    void ReturnPaper()
    {
        if(Allinfo != null)
        {
            Allinfo.style.display = DisplayStyle.None;
        }
    }
}
