using UnityEngine;
using TMPro;
using System.Collections;

public class CharacterView : MonoBehaviour
{
    public CharacterInstance charInst { get; private set; }

    public SpriteRenderer spriteRenderer;

    public TMP_Text nameDisplay;

    public TMP_Text hpDisplay;

    public CharacterInstance cachedInstance;
    

    public void ApplyDataAndVisuals(CharacterInstance instance)
    {

        Debug.Log("Character view is now applying details to the empty char.");

        spriteRenderer.sprite = instance.InGamesprite;

        nameDisplay.text = instance.charName;

        this.gameObject.name = instance.charName;

        cachedInstance = instance;

    }

    public void Update()
    {
        hpDisplay.text = cachedInstance.health.ToString();

        if (cachedInstance.health < 0)
        {
            Vulnerable();
        }

    }

    public void Vulnerable()
    {
        ShakeCoroutine(2.5f, 3.0f);

        spriteRenderer.color = Color.red;
    }

    IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        Vector2 originalPos = transform.localPosition;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * intensity;
            float y = UnityEngine.Random.Range(-1f, 1f) * intensity;

            transform.localPosition = originalPos + new Vector2(x, y);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;


    }
}
