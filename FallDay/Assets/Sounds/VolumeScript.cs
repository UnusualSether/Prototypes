using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class VolumeScript : MonoBehaviour
{
    [SerializeField] private AudioMixer volumeControl;
    [SerializeField] private UIDocument UIDocument;
    [SerializeField] private string sliderName = "Volume";

    private Slider _Volume;

    private void OnEnable()
    {

        var root = UIDocument.rootVisualElement;

        _Volume = root.Q<Slider>("MasterVolume");

        if (_Volume != null)
        {
            _Volume.RegisterValueChangedCallback(evt => SetVolume(evt.newValue));

            SetVolume(_Volume.value);
        }

    }

    public void SetVolume(float volume)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        volumeControl.SetFloat("MasterVolume", dbValue);
    }

    void OnDisable()
    {
        if (_Volume != null)
        {
            _Volume.UnregisterValueChangedCallback(evt => SetVolume(evt.newValue));
        }

    }

}
