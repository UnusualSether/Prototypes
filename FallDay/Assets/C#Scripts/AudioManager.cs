using UnityEngine;
using UnityEngine.Audio;
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

    //Teste neu audioManager



/// <summary>
/// Gerencia o volume do jogo de forma persistente entre cenas.
/// Deve existir apenas UMA instância (singleton), criada uma vez
/// (ex: na cena inicial/menu) e mantida viva com DontDestroyOnLoad.
/// </summary>
/*

    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer volumeControl;

    private const string MasterKey = "MasterVol";
    private const string MusicKey = "MusicVol";
    private const string SFXKey = "SFXVol";

    private const string MasterParam = "MasterVolume";
    private const string MusicParam = "MusicVolume";
    private const string SFXParam = "SFXVolume";

    private const float MutedDb = -80f;

    public bool IsMuted { get; private set; }

    /// <summary>Disparado quando o estado de mute muda, para qualquer UI atualizar seus sliders.</summary>
    public event System.Action<bool> OnMuteChanged;

    private void Awake()
    {
        // Garante que só existe uma instância viva; destrói duplicatas
        // (ex: se essa cena com o AudioManager for recarregada por engano)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Aplica o volume salvo (ou 1.0 na primeira vez que o jogo roda)
        // imediatamente, antes de qualquer UI abrir. É isso que garante
        // que o áudio real já bate com o valor default/salvo desde o início.
        ApplyToMixer(MasterParam, GetMasterVolume());
        ApplyToMixer(MusicParam, GetMusicVolume());
        ApplyToMixer(SFXParam, GetSFXVolume());
    }

    #region Leitura dos valores salvos (usados pela UI para posicionar os sliders)
    public float GetMasterVolume() => PlayerPrefs.GetFloat(MasterKey, 1f);
    public float GetMusicVolume() => PlayerPrefs.GetFloat(MusicKey, 1f);
    public float GetSFXVolume() => PlayerPrefs.GetFloat(SFXKey, 1f);
    #endregion

    #region Alteração de volume (chamado pelos sliders)
    public void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat(MasterKey, volume);
        if (IsMuted) return; // enquanto mutado, só guarda o valor, não toca o mixer
        ApplyToMixer(MasterParam, volume);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(MusicKey, volume);
        if (IsMuted) return;
        ApplyToMixer(MusicParam, volume);
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat(SFXKey, volume);
        if (IsMuted) return;
        ApplyToMixer(SFXParam, volume);
    }

    private void ApplyToMixer(string param, float linearVolume)
    {
        // Converte de 0-1 (linear, o que o Slider usa) para dB (o que o AudioMixer espera)
        float dB = Mathf.Log10(Mathf.Clamp(linearVolume, 0.0001f, 1f)) * 20f;
        volumeControl.SetFloat(param, dB);
    }
    #endregion

    #region Mute
    public void ToggleMute()
    {
        if (IsMuted) Unmute();
        else Mute();
    }

    // Zera o áudio (silêncio total) sem apagar os valores salvos dos sliders
    public void Mute()
    {
        if (IsMuted) return;
        IsMuted = true;

        volumeControl.SetFloat(MasterParam, MutedDb);
        volumeControl.SetFloat(MusicParam, MutedDb);
        volumeControl.SetFloat(SFXParam, MutedDb);

        OnMuteChanged?.Invoke(true);
    }

    // Restaura o áudio para os últimos valores salvos (posição em que os sliders estavam)
    public void Unmute()
    {
        if (!IsMuted) return;
        IsMuted = false;

        ApplyToMixer(MasterParam, GetMasterVolume());
        ApplyToMixer(MusicParam, GetMusicVolume());
        ApplyToMixer(SFXParam, GetSFXVolume());

        OnMuteChanged?.Invoke(false);
    }
    #endregion

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
*/
}
