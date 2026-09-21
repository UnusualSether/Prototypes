using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    // The FUCK is this? 
    public SpriteRenderer spriteRenderer;

    public void ApplyDataAndVisuals(CharacterInstance instance)
    {
        spriteRenderer.sprite = instance.data.characterSprite;

    }
}
