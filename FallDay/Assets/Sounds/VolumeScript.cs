using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class VolumeScript : MonoBehaviour
{
    [SerializeField] private AudioMixer volumeControl;
    [SerializeField] private UIDocument UIDocument;

    private Slider _Masterslider;
    private Slider _Musicslider;

    private void OnEnable()
    {

        var root = UIDocument.rootVisualElement;

        _Masterslider = root.Q<Slider>("MasterVolume");
        _Musicslider = root.Q<Slider>("MusicVolume");

        if (_Masterslider != null)
        {
            _Masterslider.RegisterValueChangedCallback(evt => MainVolume(evt.newValue));

            MainVolume(_Masterslider.value);
        }

        if (_Musicslider != null)
        {
            _Musicslider.RegisterValueChangedCallback(evt => MusicVolume(evt.newValue));

            MusicVolume(_Musicslider.value);
        }

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

    void OnDisable()
    {
        if (_Masterslider != null)
        {
            _Masterslider.UnregisterValueChangedCallback(evt => MainVolume(evt.newValue));
        }

        if (_Musicslider != null)
        {
            _Musicslider.UnregisterValueChangedCallback(evt => MusicVolume(evt.newValue));
        }

    }

}
