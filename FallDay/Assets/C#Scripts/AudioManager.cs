using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Configurações de Áudio")]
    [Tooltip("Fonte de áudio ")]
    public AudioSource sfxSource;

    [Header("Efeitos da Sequência")]
    [Tooltip("Toca com cada peça conectada")]
    public AudioClip tickConexaoClip;

    [Tooltip("Quando o jogador solta o dedo e o dano é computado")]
    public AudioClip sucessoLigacaoClip;

    [Tooltip("Qunando o jogador solta o dedo com peças erradas ou faltado")]
    public AudioClip erroLigacaoClip;

    private GameHandler gameHandler;

    void Start()
    {
        gameHandler = Object.FindFirstObjectByType<GameHandler>();

        if (gameHandler != null)
        {
            //Som enquanto está arrastando o dedo
            gameHandler.PieceConnected += TocarTickConexao;

            //Som para sucesso na ligação
            gameHandler.SucessfulShot += TocarSucessoLigacao;

            //Som para quando soltar errado 
            gameHandler.FailedShot += TocarErroLigacao;
        }
    }

    void OnDestroy()
    {
        // Limpar memoria
        if (gameHandler != null)
        {
            gameHandler.PieceConnected -= TocarTickConexao;
            gameHandler.SucessfulShot -= TocarSucessoLigacao;
            gameHandler.FailedShot -= TocarErroLigacao;
        }
    }

    

    private void TocarTickConexao()
    {
        // Usa um pitch levemente aleatório, possivel config
        sfxSource.pitch = Random.Range(0.95f, 1.05f);
        sfxSource.PlayOneShot(tickConexaoClip);
    }

    private void TocarSucessoLigacao(int dano, Zombie alvo)
    {
        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(sucessoLigacaoClip);
    }

    private void TocarErroLigacao()
    {
        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(erroLigacaoClip);
    }
}