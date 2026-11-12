using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using BetterSceneManagement;

public class FinalSceneManager : MonoBehaviour
{

    public TMP_Text somethings_text;

    public GameObject DiscScratchObj;

    public Button return_button;

    public void Start()
    {
        StartCoroutine(TextFadeIn(somethings_text, 5f, 5f));
    }

    public IEnumerator TextFadeIn(TMP_Text to_fade_in, float delay_before_fade_in, float fade_magnitude)
    {

        // Start fully transparent
        Color c = to_fade_in.color;
        c.a = 0f;
        to_fade_in.color = c;

        // Wait once, before starting
        yield return new WaitForSeconds(delay_before_fade_in);

        // Fade over time
        float elapsed = 0f;
        while (elapsed < fade_magnitude)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fade_magnitude);
            to_fade_in.color = c;

            yield return null; // pause until next frame
        }

        // Make sure it ends exactly at full opacity
        c.a = 1f;
        to_fade_in.color = c;

        yield return new WaitForSeconds(6f);

        DiscScratchText();


    }

    public void ReturnToMainMenu()
    {
        BetterSceneManager.LoadScene("MainMenu", AnimationType.Stairs);
    }

    public void DiscScratchText()
    {
        DiscScratchObj.SetActive(true);

        StartCoroutine(Shake(DiscScratchObj.gameObject, 3f, 0.6f));
    }


    public IEnumerator Shake(GameObject to_shake, float magnitude, float timer)
    {
        var elapsed = 0f;

        var starting_pos = to_shake.transform.position;

        while (elapsed < timer)
        {
            elapsed += Time.deltaTime;

            Vector2 offset = Random.insideUnitCircle * magnitude;

            to_shake.transform.position = starting_pos += (Vector3) offset;

            yield return null;

        }

        to_shake.transform.position = starting_pos;


    }
}

