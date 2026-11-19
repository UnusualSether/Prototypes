using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UISoundManager;

[RequireComponent(typeof(AudioSource))]

public class UISoundManager : MonoBehaviour
{

    [System.Serializable]
    public class SoudSet
    {
        public string name;
        public AudioClip clip;
        [Range(0f,1f)]public float volume = 1f;
        [Range(0f,0.3f)]public float pitchVariation = 1f;
    }
    [Header("Sons")]
    [SerializeField] private List<SoudSet> sounds = new List<SoudSet>();

    private AudioSource _source;
    private Dictionary<string, SoudSet> _lookup;

    void Awake()
    {
       _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;

        _lookup = new Dictionary<string, SoudSet>();
        foreach (var s in sounds)
            if (!string.IsNullOrEmpty(s.name)) _lookup[s.name] = s;

    }

   public void Play(string soundName)
    {
        if (!_lookup.TryGetValue(soundName, out var s) || s.clip == null) return;

        _source.pitch = 1f + Random.Range(-s.pitchVariation, s.pitchVariation);
        _source.PlayOneShot(s.clip, s.volume);
    }

    // Registra o som em um botão (dispara no clique)
    public void Bind(Button button, string soundName)
    {
        if (button == null) return;
        button.RegisterCallback<ClickEvent>(_ => Play(soundName));
    }



}
