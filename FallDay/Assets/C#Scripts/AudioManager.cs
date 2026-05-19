using UnityEngine;
using UnityEngine.SceneManagement; 

public class AudioManager : MonoBehaviour
{
    //Singleton para garantir que exista apenas UM AudioManager 
    public static AudioManager Instance;

    [Header("Configurações de Áudio")]
    [Tooltip("Local AudioManager")]
    public AudioSource sfxSource;

    [Header("Efeitos de conexão")]
    [Tooltip("Toca ao conectar cada peça")]
    public AudioClip linkClick;

    [Tooltip("Ativa ao Completar uma sequência")]
    public AudioClip shootSucess;

    [Tooltip("Falha de sequência")]
    public AudioClip linkFail;

    private GameHandler gameHandler;

    void Awake()
    {
        //Esse audio Manager é unico em cena,
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destruimos ele para não duplicar o som
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        // Identifica o inicio de uma nova cena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Limpa eventos antigos (Safe)
        DesinscreverEventos();

        
        gameHandler = Object.FindFirstObjectByType<GameHandler>();

        if (gameHandler != null)
        {
            
            gameHandler.PieceConnected += TocarTickConexao;
            gameHandler.SucessfulHit += TocarSucessoLigacao; 
            gameHandler.FailedShot += TocarErroLigacao;
        }
    }

    void OnDestroy()
    {
        DesinscreverEventos();
    }

    private void DesinscreverEventos()
    {
        if (gameHandler != null)
        {
            gameHandler.PieceConnected -= TocarTickConexao;
            gameHandler.SucessfulHit -= TocarSucessoLigacao;
            gameHandler.FailedShot -= TocarErroLigacao;
        }
    }

    private void TocarTickConexao()
    {
       
        sfxSource.pitch = Random.Range(0.95f, 1.05f);
        sfxSource.PlayOneShot(linkClick);
        //aqui usamos o pitch, para criar uma aleatoriedade no som para tirar a robotização dele.
    }

    private void TocarSucessoLigacao(int dano) //Recebe o dano, podendo ser usado para trabalhar o volume do tiro com base no dano obtido
    {
        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(shootSucess);
    }

    private void TocarErroLigacao()
    {
        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(linkFail);
    }
}