using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementManipulation
{
    public static void AddStatusIcon(VisualElement element, Texture icon, string name_of_effect)
    {
        var status_area  = element.Q<VisualElement>("status_area");

        var new_icon = new Image { name = name_of_effect, image = icon, tintColor = Color.red };
        new_icon.style.width = 122;
        new_icon.style.height = 86;


        status_area.Add(new_icon);
    }

    public static void ClearStatusIcon(VisualElement element)
    {
        var status_area = element.Q<VisualElement>("status_area");

        status_area.Clear();
    }
}
