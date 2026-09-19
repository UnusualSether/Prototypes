using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using UnityEditor;

public class TrinketVisual
{
    public static void IMG(Image Trinketimage, Trinket TrinketVisual)
    {
        if (Trinketimage == null || TrinketVisual == null) return;

        Trinketimage.style.display = DisplayStyle.Flex;

        //sets animation velocity and max size
        if (TrinketVisual.trinket_sprite != null && TrinketVisual.trinket_sprite.Length > 0)
        {
            int fps = 24;
            long delay = 1000 / fps;
            int index = 0;

            Sprite[] frames = TrinketVisual.trinket_sprite;

            float maxWidth = 150f;
            float maxHeight = 117f;

            foreach(var frame in frames)
            {
                if (frame == null) continue;
                if (frame.rect.width > maxWidth) maxWidth = frame.rect.width;
                if (frame.rect.width > maxHeight) maxHeight = frame.rect.height;

            }

            UpdatedFrame(Trinketimage, frames[0], maxWidth, maxHeight);

            Trinketimage.schedule.Execute(() =>
            {
                if (Trinketimage == null) return;

                UpdatedFrame(Trinketimage, frames[index], maxWidth, maxHeight);
                index = (index + 1) % frames.Length;
            }).Every(delay);

        }


    }

    private static void UpdatedFrame(VisualElement element, Sprite sprite, float maxWidth, float maxHeight)
    {
        if (sprite == null) return;

        element.style.backgroundImage = new StyleBackground(sprite);

        float scaleX = sprite.rect.width / maxWidth;
        float scaleY = sprite.rect.height / maxHeight;

        element.style.scale = new Scale(new Vector2(scaleX, scaleY));
    }

    //Animates Sprite
    private static Sprite[] GetFramesFromClip(AnimationClip clip)
    {
        EditorCurveBinding[] bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
        foreach (var binding in bindings)
        {
            if(binding.propertyName == "m_Sprite")
            {
                ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                Sprite[] sprites = new Sprite[keyframes.Length];
                for (int i = 0; i < keyframes.Length; i++)
                {
                    sprites[i] = keyframes[i].value as Sprite;
                }
                return sprites;
            }
        }
        return null;
    }

}
