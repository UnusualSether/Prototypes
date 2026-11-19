using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Lock : MonoBehaviour, IPointerClickHandler
{

    public bool is_shaking = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!is_shaking)
        {
            StartCoroutine(ShakeLock(this.gameObject, 4f, 0.4f));
            is_shaking = true;
        }

        
    }

    public IEnumerator ShakeLock(GameObject target, float magnitude, float dur)
    {
        var start_pos = target.transform.position;

        var elapsed = 0f;


        while (elapsed < dur)
        {

            elapsed += Time.deltaTime;

            target.transform.position = start_pos + (Vector3)Random.insideUnitCircle * magnitude;

            yield return null;
        }

        target.transform.position = start_pos;

        is_shaking = false;
    }

}
