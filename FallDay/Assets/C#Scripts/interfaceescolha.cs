using UnityEngine;

public class InterfaceEscolhaSala : MonoBehaviour
{
    [Header("Configuração de Referências")]
    [Tooltip("Arraste o GameHandler do seu cenário aqui")]
    public GameHandler gameHandler;

    [Tooltip("Arraste o painel ou objeto do Canvas que contém as opções visualmente")]
    public GameObject painelCanvas;

    private void OnEnable()
    {
        if (gameHandler != null)
        {
            // Se inscreve diretamente no evento global de fim de wave de zumbis
            gameHandler.PlayerKilledAllZombies += MostrarInterface;
        }
    }

    private void OnDisable()
    {
        if (gameHandler != null)
        {
            gameHandler.PlayerKilledAllZombies -= MostrarInterface;
        }
    }

    private void Start()
    {
        // Garante que o Canvas comece escondido ao iniciar o jogo
        if (painelCanvas != null)
        {
            painelCanvas.SetActive(false);
        }
    }

    private void MostrarInterface()
    {
        if (painelCanvas != null)
        {
            painelCanvas.SetActive(true);
            Debug.Log("Interface de escolha ativada via evento independente.");
        }
    }

    // Você pode linkar este método nos botões de escolha do Canvas (On Click) 
    // para fechar a interface assim que o jogador decidir o caminho
    public void EsconderInterface()
    {
        if (painelCanvas != null)
        {
            painelCanvas.SetActive(false);
        }
    }
}