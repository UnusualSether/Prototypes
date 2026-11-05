using UnityEngine;
using UnityEngine.UIElements;

/// <summary>Funções de apoio usadas por vários painéis.</summary>
public static class UIUtils
{
    // Liga o som a um botão (ignora se o som ou o botão não existirem)
    public static void BindSound(UISoundManager sound, Button button, string soundId)
    {
        if (sound != null && button != null)
            sound.Bind(button, soundId);
    }

    // Efeito de pulsar (escala oscilando)
    public static void Pulse(VisualElement el, float min, float max, float speed)
    {
        if (el == null) return;

        el.schedule.Execute(() =>
        {
            float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
            float s = Mathf.Lerp(min, max, t);
            el.style.scale = new Scale(new Vector3(s, s, 1f));
        }).Every(16);
    }
}
