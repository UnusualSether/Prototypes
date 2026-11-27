using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Liga os 3 sliders (Master, Music, SFX) do UI Toolkit aos volumes do jogo.
/// Use um componente para o menu principal (containerName = SoundController)
/// e outro para o menu de pausa (containerName = ScreenSetting).
/// Os dois ficam sincronizados e o valor é salvo.
/// </summary>
public class AudioSettingsPanel : MonoBehaviour
{
    [SerializeField] private UIDocument document;

    [Tooltip("Nome do elemento que contém os 3 sliders. Vazio = documento inteiro.")]
    [SerializeField] private string containerName = "";

    [SerializeField] private string masterName = "MasterVolume";
    [SerializeField] private string musicName = "MusicVolume";
    [SerializeField] private string sfxName = "SFXVolume";

    private Slider master, music, sfx;

    private void OnEnable()
    {
        var root = document.rootVisualElement;
        var scope = string.IsNullOrEmpty(containerName) ? root : root.Q(containerName);
        if (scope == null)
        {
            Debug.LogWarning($"[Audio] Container '{containerName}' não encontrado no UIDocument.", this);
            return;
        }

        master = Setup(scope, masterName, OnMaster);
        music = Setup(scope, musicName, OnMusic);
        sfx = Setup(scope, sfxName, OnSfx);

        GameAudioSettings.Changed += Sync;
        Sync();
    }

    private void OnDisable()
    {
        GameAudioSettings.Changed -= Sync;
        master?.UnregisterValueChangedCallback(OnMaster);
        music?.UnregisterValueChangedCallback(OnMusic);
        sfx?.UnregisterValueChangedCallback(OnSfx);
        GameAudioSettings.Save();
    }

    private static Slider Setup(VisualElement scope, string sliderName, EventCallback<ChangeEvent<float>> callback)
    {
        var slider = scope.Q<Slider>(sliderName);
        if (slider == null)
        {
            Debug.LogWarning($"[Audio] Slider '{sliderName}' não encontrado.");
            return null;
        }

        slider.lowValue = 0f;
        slider.highValue = 1f;
        slider.RegisterValueChangedCallback(callback);
        return slider;
    }

    private void OnMaster(ChangeEvent<float> e) => GameAudioSettings.Master = e.newValue;
    private void OnMusic(ChangeEvent<float> e) => GameAudioSettings.Music = e.newValue;
    private void OnSfx(ChangeEvent<float> e) => GameAudioSettings.Sfx = e.newValue;

    private void Sync()
    {
        master?.SetValueWithoutNotify(GameAudioSettings.Master);
        music?.SetValueWithoutNotify(GameAudioSettings.Music);
        sfx?.SetValueWithoutNotify(GameAudioSettings.Sfx);
    }
}
