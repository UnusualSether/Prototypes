using UnityEngine;
using UnityEngine.UI; // Necessário para elementos do Canvas

public class EfeitoTiroCanvas : MonoBehaviour
{
    [Header("Conexões")]
    public GameHandler gameHandler;

    [Tooltip("Arraste o Canvas ou o Painel do Canvas onde os tiros devem aparecer")]
    public RectTransform painelCanvas;

    [Header("Configuração da Animação (UI)")]
    [Tooltip("O Prefab do tiro que contém um componente 'Image' e 'Animator'")]
    public GameObject prefabAnimacaoTiroUI;

    [Tooltip("Tempo até sumir da tela")]
    public float duracaoDaAnimacao = 0.5f;

    private void OnEnable()
    {
        if (gameHandler != null)
        {
            gameHandler.ZombieDamaged += GatilhoDoTiro;
        }
    }

    private void OnDisable()
    {
        if (gameHandler != null)
        {
            gameHandler.ZombieDamaged -= GatilhoDoTiro;
        }
    }

    private void GatilhoDoTiro(Zombie zombie)
    {
        PuxarAnimacaoGuardada();
    }

    public void PuxarAnimacaoGuardada()
    {
        if (prefabAnimacaoTiroUI == null || painelCanvas == null) return;

        // 1. "Puxa" a animação instanciando o Prefab como filho do Canvas
        GameObject novoTiro = Instantiate(prefabAnimacaoTiroUI, painelCanvas);
        RectTransform rectTiro = novoTiro.GetComponent<RectTransform>();

        // 2. Descobre o tamanho atual do seu Canvas/Painel para não sair da tela
        float metadeLargura = painelCanvas.rect.width / 2f;
        float metadeAltura = painelCanvas.rect.height / 2f;

        // Margem de segurança (em pixels) para o tiro não ficar cortado na borda
        float margem = 50f;

        // 3. Sorteia a posição X e Y baseado no centro do Canvas (0,0)
        float randomX = Random.Range(-metadeLargura + margem, metadeLargura - margem);
        float randomY = Random.Range(-metadeAltura + margem, metadeAltura - margem);

        // 4. Aplica a posição aleatória ao UI
        rectTiro.anchoredPosition = new Vector2(randomX, randomY);

        // 5. Destrói a cópia após a animação acabar
        Destroy(novoTiro, duracaoDaAnimacao);
    }
}