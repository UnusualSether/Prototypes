using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Timeline;

public partial class GameDisplay
{
    //In this script, you'll find code relating to cast shadows on bullet icons and their accompanying animations & sounds which add flavour
    //and player feedback to the Match 3 system



    [Header("Icon Animations")]
    [SerializeField]
    float bulletShakeDuration = 0.5f;
    [SerializeField]
    float bulletShakeMagnitude = 0.5f;

    [Header("Camera Animations")]
    [SerializeField]
    float screenShakeDuration = 0.5f;
    [SerializeField]
    float screenShakeMagnitude = 0.5f;
    

    void ShakeBullet(VisualElement elementToShake)
    {
        if (elementToShake == null)
        {
            return;
        }

       //Check if readyBullets has any elements just so the multiplication doesn't return a zero.
        if (handler.readyBullets.Count > 0)
        {
            StartCoroutine(ShakeElement(elementToShake, bulletShakeDuration, bulletShakeMagnitude * handler.readyBullets.Count));
        }

        //If it is zero then just use the base multiplier.
        else
            StartCoroutine(ShakeElement(elementToShake, bulletShakeDuration, bulletShakeMagnitude));


    }

    void ShakeScreen(int damageDealt)
    {
        if (damageDealt == 0)
        {
            return;
        }

        StartCoroutine(ShakeScreen(Camera.main, screenShakeDuration, screenShakeMagnitude));


    }

    private static IEnumerator ShakeElement(VisualElement elementToShake, float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Magnitude decreases over time for a natural effect
            float progress = elapsed / duration;
            float currentMagnitude = magnitude * (1f - progress);

            float randomX = Random.Range(-currentMagnitude, currentMagnitude);
            float randomY = Random.Range(-currentMagnitude, currentMagnitude);

            elementToShake.style.translate = new StyleTranslate(new Translate(randomX, randomY));

            elapsed += Time.deltaTime;
            yield return null;
        }

        elementToShake.style.translate = new StyleTranslate(StyleKeyword.None);
    }

    private static IEnumerator ShakeScreen(Camera camera, float duration, float magnitude)
    {
        Vector3 originalPosition = camera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Random offset within magnitude range
            float randomX = Random.Range(-magnitude, magnitude);
            float randomY = Random.Range(-magnitude, magnitude);
            float randomZ = Random.Range(-magnitude, magnitude);

            // Apply shake offset
            camera.transform.localPosition = originalPosition + new Vector3(randomX, randomY, randomZ);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to original position
        camera.transform.localPosition = originalPosition;
    }



    [Header("Audio")]
    public AudioClip clickSound;
    public AudioSource audioSource;

    void PlayerClickSound(VisualElement element)
    {
        if (handler.readyBullets.Count < 0)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(clickSound);
        }

        else
        {
            audioSource.pitch = audioSource.pitch + handler.readyBullets.Count;
            audioSource.PlayOneShot(clickSound);
            audioSource.pitch = 1f;
        }
    }
}



