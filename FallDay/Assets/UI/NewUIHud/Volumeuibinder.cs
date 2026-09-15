using UnityEngine;
using UnityEngine.UIElements;
/*
/// <summary>
/// Conecta os sliders de volume da UIDocument desta cena ao AudioManager
/// (que é quem de fato guarda/aplica o volume, de forma persistente).
/// Coloque este script no mesmo objeto da UIDocument que tem os sliders
/// (ex: o painel de Setting do pause).
/// </summary>
public class VolumeUIBinder : MonoBehaviour
{
    [SerializeField] private UIDocument UIDocument;

    // Ajuste este nome para o nome real do botão de mute na sua UXML
    [SerializeField] private string muteButtonName = "MuteButton";

    private Slider _masterSlider, _musicSlider, _sfxSlider;
    private Button _muteButton;

    private void OnEnable()
    {
        var root = UIDocument.rootVisualElement;

        _masterSlider = root.Q<Slider>("MasterVolume");
        _musicSlider = root.Q<Slider>("MusicVolume");
        _sfxSlider = root.Q<Slider>("SFXVolume");
        _muteButton = root.Q<Button>(muteButtonName);

        var audio = AudioManager.Instance;
        if (audio == null)
        {
            Debug.LogWarning("AudioManager não encontrado. Verifique se ele existe na cena inicial e está marcado com DontDestroyOnLoad.");
            return;
        }

        // Posiciona os sliders no valor salvo (ou 1.0 na primeira execução),
        // sem disparar o callback de mudança (SetValueWithoutNotify),
        // já que isso é só sincronizar a UI com o áudio, não uma mudança do usuário.
        if (_masterSlider != null) _masterSlider.SetValueWithoutNotify(audio.GetMasterVolume());
        if (_musicSlider != null) _musicSlider.SetValueWithoutNotify(audio.GetMusicVolume());
        if (_sfxSlider != null) _sfxSlider.SetValueWithoutNotify(audio.GetSFXVolume());

        if (_masterSlider != null) _masterSlider.RegisterValueChangedCallback(OnMasterChanged);
        if (_musicSlider != null) _musicSlider.RegisterValueChangedCallback(OnMusicChanged);
        if (_sfxSlider != null) _sfxSlider.RegisterValueChangedCallback(OnSFXChanged);
        if (_muteButton != null) _muteButton.RegisterCallback<ClickEvent>(OnMuteClicked);

        audio.OnMuteChanged += OnMuteStateChanged;
        // Se o jogo já estiver mutado quando essa tela abrir, reflete isso de imediato
        OnMuteStateChanged(audio.IsMuted);
    }

    private void OnDisable()
    {
        if (_masterSlider != null) _masterSlider.UnregisterValueChangedCallback(OnMasterChanged);
        if (_musicSlider != null) _musicSlider.UnregisterValueChangedCallback(OnMusicChanged);
        if (_sfxSlider != null) _sfxSlider.UnregisterValueChangedCallback(OnSFXChanged);
        if (_muteButton != null) _muteButton.UnregisterCallback<ClickEvent>(OnMuteClicked);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.OnMuteChanged -= OnMuteStateChanged;
        }

        PlayerPrefs.Save();
    }

    private void OnMasterChanged(ChangeEvent<float> evt) => AudioManager.Instance.SetMasterVolume(evt.newValue);
    private void OnMusicChanged(ChangeEvent<float> evt) => AudioManager.Instance.SetMusicVolume(evt.newValue);
    private void OnSFXChanged(ChangeEvent<float> evt) => AudioManager.Instance.SetSFXVolume(evt.newValue);

    // Botão "X" de mute: apenas alterna o estado no AudioManager
    private void OnMuteClicked(ClickEvent evt)
    {
        AudioManager.Instance.ToggleMute();
    }

    // Quando o mute muda (por este botão ou por qualquer outra UI), reflete
    // visualmente nos sliders: zera tudo ao mutar, restaura ao desmutar.
    private void OnMuteStateChanged(bool isMuted)
    {
        var audio = AudioManager.Instance;

        if (isMuted)
        {
            if (_masterSlider != null) _masterSlider.SetValueWithoutNotify(0f);
            if (_musicSlider != null) _musicSlider.SetValueWithoutNotify(0f);
            if (_sfxSlider != null) _sfxSlider.SetValueWithoutNotify(0f);
        }
        else
        {
            if (_masterSlider != null) _masterSlider.SetValueWithoutNotify(audio.GetMasterVolume());
            if (_musicSlider != null) _musicSlider.SetValueWithoutNotify(audio.GetMusicVolume());
            if (_sfxSlider != null) _sfxSlider.SetValueWithoutNotify(audio.GetSFXVolume());
        }
    }
}
*/