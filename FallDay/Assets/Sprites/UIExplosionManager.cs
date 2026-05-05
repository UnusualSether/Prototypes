using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class UIVFXManager : MonoBehaviour
{
    private UIDocument uiDocument;

    // Array para guardar as telas de explo~são
    private VisualElement[] enemyVFXSlots = new VisualElement[4];

    [Header("Animações")]
    public Sprite[] explosaoTiroFrames;
    public float frameRate = 0.05f;

    private GameHandler gameHandler;

    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        // Busca os  VFX no UI Builder baseados no nome
        for (int i = 0; i < 4; i++)
        {
            enemyVFXSlots[i] = root.Q<VisualElement>($"VFX_Zombie_{i}");
            if (enemyVFXSlots[i] != null)
            {
                enemyVFXSlots[i].style.display = DisplayStyle.None;
            }
        }

        // Inscreve no evento do GameHandler
        gameHandler = Object.FindFirstObjectByType<GameHandler>();
        if (gameHandler != null)
        {
            gameHandler.SucessfulShot += IniciarAnimacaoAleatoria;
        }
    }

    void OnDestroy()
    {
        if (gameHandler != null)
        {
            gameHandler.SucessfulShot -= IniciarAnimacaoAleatoria;
        }
    }

    
    private void IniciarAnimacaoAleatoria(int dano, Zombie alvo)
    {
        // Sorteia um número de 0 a 3 
        int indexAleatorio = Random.Range(0, 4);

        VisualElement telaSorteada = enemyVFXSlots[indexAleatorio];

        if (telaSorteada != null)
        {
            Debug.Log($"[VFX Manager] Sorteado alvo aleatório: {telaSorteada.name}");
            StartCoroutine(PlayExplosionAnimation(telaSorteada));
        }
        else
        {
            Debug.LogWarning($"[VFX Manager] A tela sorteada (Index {indexAleatorio}) está nula!");
        }
    }

    private IEnumerator PlayExplosionAnimation(VisualElement targetElement)
    {
        
        targetElement.style.display = DisplayStyle.Flex;

        foreach (Sprite frame in explosaoTiroFrames)
        {
            targetElement.style.backgroundImage = new StyleBackground(frame);
            yield return new WaitForSecondsRealtime(frameRate);
        }

        
        targetElement.style.display = DisplayStyle.None;
    }
}