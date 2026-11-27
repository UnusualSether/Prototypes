using System;
using UnityEngine;

/// <summary>
/// Volumes escolhidos pelo jogador (0 a 1), salvos em PlayerPrefs.
/// Todos os sliders (menu e pausa) usam esta classe, então ficam sempre sincronizados.
/// </summary>
public static class GameAudioSettings
{
    private const string MasterKey = "MasterVol";
    private const string MusicKey = "MusicVol";
    private const string SfxKey = "SFXVol";

    private static float master = -1f, music = -1f, sfx = -1f;

    public static event Action Changed;

    public static float Master
    {
        get => Load(ref master, MasterKey, 1f);
        set => Store(ref master, MasterKey, value);
    }

    public static float Music
    {
        get => Load(ref music, MusicKey, 1f);
        set => Store(ref music, MusicKey, value);
    }

    public static float Sfx
    {
        get => Load(ref sfx, SfxKey, 1f);
        set => Store(ref sfx, SfxKey, value);
    }

    // Ganho final aplicado nos AudioSources. A curva quadrática faz o slider "soar" mais natural.
    // Com AudioMixer ligado no AudioManager (MixerMode), o mixer já controla o volume em dB,
    // então o ganho aqui vira 1 para não aplicar o volume duas vezes.
    public static bool MixerMode;

    public static float MusicGain => MixerMode ? 1f : Curve(Master) * Curve(Music);
    public static float SfxGain => MixerMode ? 1f : Curve(Master) * Curve(Sfx);

    // Slider linear (0-1) para dB, o que o AudioMixer espera
    public static float ToDecibels(float linear) => Mathf.Log10(Mathf.Clamp(linear, 0.0001f, 1f)) * 20f;

    public static void Save() => PlayerPrefs.Save();

    private static float Curve(float v) => v * v;

    private static float Load(ref float field, string key, float defaultValue)
    {
        if (field < 0f) field = PlayerPrefs.GetFloat(key, defaultValue);
        return field;
    }

    private static void Store(ref float field, string key, float value)
    {
        value = Mathf.Clamp01(value);
        if (Mathf.Approximately(field, value)) return;
        field = value;
        PlayerPrefs.SetFloat(key, value);
        Changed?.Invoke();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        master = music = sfx = -1f;
        Changed = null;
        MixerMode = false;
    }
}
