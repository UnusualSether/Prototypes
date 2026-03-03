using UnityEngine;

public class PlayerCard : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;

    public void ApplyDataAndVisuals(CharacterInstance instance)
    {
        spriteRenderer.sprite = instance.data.characterSprite;

    }
}
