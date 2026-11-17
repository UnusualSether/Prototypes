using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public enum HapticStrength { Light = 0, Medium = 1 }


public static class Haptics
{
    // ---------- Ajustes de intensidade (mexa aqui) ----------
    private const long LightMs = 20;          // toque em botão
    private const int LightAmplitude = 80;    // 1 a 255 (só Android 8+)
    private const long MediumMs = 40;         // início de animação de painel
    private const int MediumAmplitude = 160;

    private const string PrefKey = "HapticsEnabled";
    private const float MinInterval = 0.05f;  // evita vibrações coladas

    private static bool _loaded;
    private static bool _enabled = true;
    private static float _lastTime = -10f;
    private static HapticStrength _lastStrength;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        _loaded = false;
        _lastTime = -10f;
    }

    private static void Load()
    {
        if (_loaded) return;
        _enabled = PlayerPrefs.GetInt(PrefKey, 1) == 1;
        _loaded = true;
    }

    /// <summary>Liga/desliga a vibração (salvo entre sessões).</summary>
    public static bool Enabled
    {
        get { Load(); return _enabled; }
        set
        {
            Load();
            _enabled = value;
            PlayerPrefs.SetInt(PrefKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static void Play(HapticStrength strength)
    {
        if (!Enabled) return;

        // Se duas vibrações caem quase juntas (clique + animação), vale a mais forte
        float now = Time.unscaledTime;
        if (now - _lastTime < MinInterval && strength <= _lastStrength) return;
        _lastTime = now;
        _lastStrength = strength;

        PlatformPlay(strength);
    }

    // ---------------------------------------------------------------
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject _vibrator;
    private static int _sdk;
    private static bool _androidReady;

    // Nunca fica true. Existe só para o Unity enxergar o uso de Handheld.Vibrate
    // e adicionar a permissão VIBRATE ao manifest automaticamente.
    private static bool _permissionHook = false;

    private static void InitAndroid()
    {
        if (_androidReady) return;
        _androidReady = true;

        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                _vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            }
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                _sdk = version.GetStatic<int>("SDK_INT");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Haptics: não foi possível iniciar o vibrador. " + e.Message);
            _vibrator = null;
        }
    }

    private static void VibrateAndroid(long ms, int amplitude)
    {
        InitAndroid();
        if (_vibrator == null) return;

        try
        {
            if (_sdk >= 26)
            {
                using (var effectClass = new AndroidJavaClass("android.os.VibrationEffect"))
                using (var effect = effectClass.CallStatic<AndroidJavaObject>("createOneShot", ms, amplitude))
                {
                    _vibrator.Call("vibrate", effect);
                }
            }
            else
            {
                _vibrator.Call("vibrate", ms);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Haptics: falha ao vibrar. " + e.Message);
        }
    }
#endif

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _HapticImpact(int style); // 0 = leve, 1 = médio
#endif

    private static void PlatformPlay(HapticStrength strength)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        long ms = strength == HapticStrength.Light ? LightMs : MediumMs;
        int amplitude = strength == HapticStrength.Light ? LightAmplitude : MediumAmplitude;

        if (_permissionHook) Handheld.Vibrate();
        VibrateAndroid(ms, amplitude);
#elif UNITY_IOS && !UNITY_EDITOR
        _HapticImpact(strength == HapticStrength.Light ? 0 : 1);
#endif
    }
}