using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// AudioManager unificado:
///  - PARTE ANTIGA (mantida): sons de conexão/tiro ligados aos eventos do GameHandler.
///  - PARTE NOVA: música com playlists/crossfade, biblioteca de SFX por id e volumes salvos.
///
/// API nova (de qualquer script):
///   AudioManager.PlayMusic(playlist);   AudioManager.StopMusic();
///   AudioManager.Play("ui_click");      AudioManager.PlayAt("explosion", transform.position);
/// </summary>
public class AudioManager : MonoBehaviour
{
    //Singleton para garantir que exista apenas UM AudioManager 
    public static AudioManager Instance { get; private set; }
    public static MusicPlaylist CurrentPlaylist => Instance != null ? Instance.currentPlaylist : null;
    public static AudioMixerGroup SfxGroup => Instance != null ? Instance.sfxGroup : null;

    // ===================== PARTE ANTIGA (campos mantidos) =====================

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

    // ===================== PARTE NOVA (campos) =====================

    [Header("Novo sistema - Bibliotecas de SFX (UI, Player, Inimigos...)")]
    [SerializeField] private List<SfxLibrary> sfxLibraries = new List<SfxLibrary>();

    [Header("Novo sistema - SFX")]
    [Tooltip("Quantos sons do pool podem tocar ao mesmo tempo.")]
    [SerializeField, Min(1)] private int sfxVoices = 16;

    [Header("Novo sistema - AudioMixer (opcional)")]
    [Tooltip("Se preenchido, o mixer controla os volumes (em dB) e os sources são roteados para os grupos abaixo. Vazio = volume aplicado direto nos AudioSources.")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private string masterParam = "MasterVolume";
    [SerializeField] private string musicParam = "MusicVolume";
    [SerializeField] private string sfxParam = "SFXVolume";

    // Música: 2 AudioSources para fazer crossfade
    private readonly AudioSource[] musicSources = new AudioSource[2];
    private readonly float[] musicFade = new float[2];
    private readonly float[] musicBase = new float[2];
    private int activeMusic;
    private MusicPlaylist currentPlaylist;
    private int trackIndex = -1;
    private Coroutine fadeRoutine;
    private float advanceLock;

    // SFX: pool de AudioSources
    private AudioSource[] sfxPool;
    private int sfxNext;
    private readonly Dictionary<string, SfxEntry> sfxLookup = new Dictionary<string, SfxEntry>();
    private readonly Dictionary<string, float> sfxLastPlayed = new Dictionary<string, float>();

    // ===================== CICLO DE VIDA =====================

    // Cria o AudioManager sozinho em qualquer cena, se existir um prefab em Resources/AudioManager.
    // (Permite dar Play direto em um level sem passar pelo menu.)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;
        var prefab = Resources.Load<AudioManager>("AudioManager");
        if (prefab != null) Instantiate(prefab);
    }

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

        InitNewAudio();
        InitMixer();
    }

    void Start()
    {
        // Caso o manager já exista quando a cena começa, liga os eventos do GameHandler dela
        if (Instance == this)
        {
            HookGameHandler();
            ApplyMixer(); // reaplica: alguns Unity ignoram SetFloat feito no Awake
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
        if (Instance != this) return; // ignora duplicatas que serão destruídas
        HookGameHandler();
    }

    private void HookGameHandler()
    {
        //Limpa eventos antigos (Safe)
        DesinscreverEventos();


        gameHandler = Object.FindFirstObjectByType<GameHandler>();

        if (gameHandler != null)
        {

            gameHandler.PieceConnected += TocarTickConexao;
            gameHandler.SucessfulShot += TocarSucessoLigacao;
            gameHandler.FailedShot += TocarErroLigacao;
        }
    }

    void OnDestroy()
    {
        DesinscreverEventos();
        if (Instance == this)
        {
            GameAudioSettings.Changed -= ApplyMixer;
            GameAudioSettings.MixerMode = false;
            Instance = null;
        }
    }

    private void DesinscreverEventos()
    {
        if (gameHandler != null)
        {
            gameHandler.PieceConnected -= TocarTickConexao;
            gameHandler.SucessfulShot -= TocarSucessoLigacao;
            gameHandler.FailedShot -= TocarErroLigacao;
        }
    }

    // ===================== PARTE ANTIGA (sons dos eventos do jogo) =====================
    // Agora também respeitam o volume de SFX e o Master escolhidos no menu.

    private void TocarTickConexao()
    {

        sfxSource.pitch = Random.Range(0.95f, 1.05f);
        sfxSource.PlayOneShot(linkClick, GameAudioSettings.SfxGain);
        //aqui usamos o pitch, para criar uma aleatoriedade no som para tirar a robotização dele.
    }

    private void TocarSucessoLigacao(int dano) //Recebe o dano, podendo ser usado para trabalhar o volume do tiro com base no dano obtido
    {
        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(shootSucess, GameAudioSettings.SfxGain);
    }

    private void TocarErroLigacao()
    {
        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(linkFail, GameAudioSettings.SfxGain);
    }

    // ===================== PARTE NOVA =====================

    private void InitNewAudio()
    {
        // Se o sfxSource não foi ligado no Inspector, cria um para não quebrar os sons antigos
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        for (int i = 0; i < 2; i++)
        {
            var go = new GameObject("Music" + i);
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 0f;
            src.outputAudioMixerGroup = musicGroup;
            musicSources[i] = src;
        }

        sfxPool = new AudioSource[sfxVoices];
        for (int i = 0; i < sfxVoices; i++)
        {
            var go = new GameObject("Sfx" + i);
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.rolloffMode = AudioRolloffMode.Linear;
            src.outputAudioMixerGroup = sfxGroup;
            sfxPool[i] = src;
        }

        foreach (var lib in sfxLibraries)
        {
            if (lib == null) continue;
            foreach (var entry in lib.entries)
            {
                if (string.IsNullOrEmpty(entry.id)) continue;
                if (sfxLookup.ContainsKey(entry.id))
                    Debug.LogWarning($"[Audio] Id de SFX repetido: '{entry.id}' (o último da lista vale).", lib);
                sfxLookup[entry.id] = entry;
            }
        }
    }

    #region Mixer (opcional)

    private void InitMixer()
    {
        if (mixer == null) return;

        GameAudioSettings.MixerMode = true;

        // Os sons antigos (sfxSource) também passam pelo grupo de SFX, se ainda não tiverem grupo
        if (sfxSource != null && sfxSource.outputAudioMixerGroup == null)
            sfxSource.outputAudioMixerGroup = sfxGroup;

        GameAudioSettings.Changed += ApplyMixer;
        ApplyMixer();
    }

    // Aplica Master/Music/SFX no mixer (em dB) no início do jogo e a cada mudança de slider,
    // não só quando o menu abre.
    private void ApplyMixer()
    {
        if (mixer == null) return;
        mixer.SetFloat(masterParam, GameAudioSettings.ToDecibels(GameAudioSettings.Master));
        mixer.SetFloat(musicParam, GameAudioSettings.ToDecibels(GameAudioSettings.Music));
        mixer.SetFloat(sfxParam, GameAudioSettings.ToDecibels(GameAudioSettings.Sfx));
    }

    #endregion

    #region API estática

    public static void PlayMusic(MusicPlaylist playlist, bool restart = false)
    {
        if (Instance != null) Instance.PlayPlaylist(playlist, restart);
    }

    public static void StopMusic(float fadeTime = 1f)
    {
        if (Instance != null) Instance.FadeOutMusic(fadeTime);
    }

    public static void Play(string id, float volumeScale = 1f)
    {
        if (Instance != null) Instance.PlaySfx(id, volumeScale, null);
    }

    public static void PlayAt(string id, Vector3 position, float volumeScale = 1f)
    {
        if (Instance != null) Instance.PlaySfx(id, volumeScale, position);
    }

    #endregion

    #region Música

    private void Update()
    {
        float gain = GameAudioSettings.MusicGain;
        for (int i = 0; i < 2; i++)
            musicSources[i].volume = musicFade[i] * musicBase[i] * gain;

        TryAdvanceTrack();
    }

    private void PlayPlaylist(MusicPlaylist playlist, bool restart)
    {
        if (playlist == null)
        {
            FadeOutMusic(1f);
            return;
        }

        if (playlist == currentPlaylist && !restart) return; // já está tocando

        currentPlaylist = playlist;
        trackIndex = -1;
        StartTrack(playlist.fadeTime);
    }

    private void StartTrack(float fadeTime)
    {
        var clip = PickNext();
        if (clip == null)
        {
            FadeOutMusic(fadeTime);
            return;
        }

        int from = activeMusic;
        int to = 1 - from;
        activeMusic = to;

        var src = musicSources[to];
        src.Stop();
        src.clip = clip;
        src.loop = currentPlaylist.tracks.Count == 1; // playlist de 1 música fica em loop
        musicBase[to] = currentPlaylist.volume;
        musicFade[to] = 0f;
        src.Play();

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(Crossfade(from, to, fadeTime));
        advanceLock = Time.unscaledTime + 0.5f;
    }

    private AudioClip PickNext()
    {
        var tracks = currentPlaylist.tracks;
        int count = tracks.Count;
        if (count == 0) return null;

        if (currentPlaylist.shuffle && count > 1)
        {
            int n;
            do { n = Random.Range(0, count); } while (n == trackIndex);
            trackIndex = n;
        }
        else
        {
            trackIndex = (trackIndex + 1) % count;
        }

        return tracks[trackIndex];
    }

    // Passa para a próxima música um pouco antes da atual acabar (crossfade)
    private void TryAdvanceTrack()
    {
        if (currentPlaylist == null || !Application.isFocused || AudioListener.pause) return;
        if (Time.unscaledTime < advanceLock) return;

        var src = musicSources[activeMusic];
        if (src.clip == null || src.loop) return;

        float fade = Mathf.Min(currentPlaylist.fadeTime, src.clip.length * 0.5f);
        bool nearEnd = src.isPlaying && src.time >= src.clip.length - fade - 0.05f;
        bool finished = !src.isPlaying;

        if (nearEnd || finished) StartTrack(fade);
    }

    private IEnumerator Crossfade(int from, int to, float time)
    {
        float startFrom = musicFade[from];
        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / time);
            musicFade[to] = k;
            musicFade[from] = startFrom * (1f - k);
            yield return null;
        }

        musicFade[to] = 1f;
        musicFade[from] = 0f;
        musicSources[from].Stop();
        fadeRoutine = null;
    }

    private void FadeOutMusic(float time)
    {
        currentPlaylist = null;
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeOutAll(time));
    }

    private IEnumerator FadeOutAll(float time)
    {
        float s0 = musicFade[0], s1 = musicFade[1];
        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            float k = 1f - Mathf.Clamp01(t / time);
            musicFade[0] = s0 * k;
            musicFade[1] = s1 * k;
            yield return null;
        }

        for (int i = 0; i < 2; i++)
        {
            musicFade[i] = 0f;
            musicSources[i].Stop();
        }
        fadeRoutine = null;
    }

    #endregion

    #region SFX por id

    private void PlaySfx(string id, float volumeScale, Vector3? position)
    {
        if (string.IsNullOrEmpty(id)) return;

        if (!sfxLookup.TryGetValue(id, out var entry))
        {
            Debug.LogWarning($"[Audio] SFX não encontrado: '{id}'");
            return;
        }
        if (entry.clips == null || entry.clips.Length == 0) return;

        float now = Time.unscaledTime;
        if (sfxLastPlayed.TryGetValue(id, out float last) && now - last < entry.cooldown) return;
        sfxLastPlayed[id] = now;

        var src = NextSfxSource();
        src.clip = entry.clips[Random.Range(0, entry.clips.Length)];
        src.pitch = Random.Range(entry.minPitch, entry.maxPitch);
        src.volume = Mathf.Clamp01(entry.volume * volumeScale * GameAudioSettings.SfxGain);
        src.minDistance = entry.minDistance;
        src.maxDistance = entry.maxDistance;

        if (position.HasValue)
        {
            src.transform.position = position.Value;
            src.spatialBlend = entry.spatialBlend;
        }
        else
        {
            src.spatialBlend = 0f;
        }

        src.Play();
    }

    private AudioSource NextSfxSource()
    {
        for (int i = 0; i < sfxPool.Length; i++)
        {
            int idx = (sfxNext + i) % sfxPool.Length;
            if (!sfxPool[idx].isPlaying)
            {
                sfxNext = (idx + 1) % sfxPool.Length;
                return sfxPool[idx];
            }
        }

        // Todos ocupados: reaproveita o mais antigo
        var s = sfxPool[sfxNext];
        sfxNext = (sfxNext + 1) % sfxPool.Length;
        return s;
    }

    #endregion

    // ===================== BLOCO ANTIGO COMENTADO (mantido sem alterações) =====================

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
