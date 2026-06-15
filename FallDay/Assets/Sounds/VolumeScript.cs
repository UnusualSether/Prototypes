using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class VolumeScript : MonoBehaviour
{
    [SerializeField] private AudioMixer volumeControl;
    [SerializeField] private UIDocument UIDocument;

    private Slider Masterslider, sfxslider, Musicslider;

    private const string Master = "MasterVol";
    private const string Music = "MusicVol";
    private const string SFX = "SFXVol";

    private void OnEnable()
    {

        var root = UIDocument.rootVisualElement;

        Masterslider = root.Q<Slider>("MasterVolume");
        Musicslider = root.Q<Slider>("MusicVolume");
        sfxslider = root.Q<Slider>("SFXVolume");

        if (Masterslider != null) Masterslider.value = PlayerPrefs.GetFloat(Master, 1.0f);
        if (Musicslider != null) Musicslider.value = PlayerPrefs.GetFloat(Music, 1.0f);
        if (sfxslider != null) sfxslider.value = PlayerPrefs.GetFloat(SFX, 1.0f);

        if (Masterslider != null) Masterslider.RegisterValueChangedCallback(MasterSaved);
        if (Musicslider != null) Musicslider.RegisterValueChangedCallback(MusicSaved);
        if (sfxslider != null) sfxslider.RegisterValueChangedCallback(SFXSaved);

        if (Masterslider != null) MainVolume(Masterslider.value);
        if (Musicslider != null) MusicVolume(Musicslider.value);
        if (sfxslider != null) SFXVolume(sfxslider.value);

    }

    public void MasterSaved(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat(Master, evt.newValue);
        MainVolume(evt.newValue);
    }

    public void MusicSaved(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat(Music, evt.newValue);
        MusicVolume(evt.newValue);
    }

    public void SFXSaved(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat(SFX, evt.newValue);
        SFXVolume(evt.newValue);
    }

    public void MainVolume(float volume)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        volumeControl.SetFloat("MasterVolume", dbValue);
    }

    public void MusicVolume(float volume)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        volumeControl.SetFloat("MusicVolume", dbValue);
    }

    public void SFXVolume(float volume)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        volumeControl.SetFloat("SFXVolume", dbValue);
    }

    void OnDisable()
    {

        PlayerPrefs.Save();

        if (Masterslider != null)
        {
            Masterslider.UnregisterValueChangedCallback(evt => MainVolume(evt.newValue));
        }

        if (Musicslider != null)
        {
            Musicslider.UnregisterValueChangedCallback(evt => MusicVolume(evt.newValue));
        }

        if (sfxslider != null)
        {
            sfxslider.UnregisterValueChangedCallback(evt => SFXVolume(evt.newValue));
        }

    }

}
