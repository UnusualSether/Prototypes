using System;
using System.Collections;
using UnityEngine;

// Class for Monster/Zomby Behavior
public class Zombi : MonoBehaviour
{
    public float HP, Attack;
    public bool corutineTimer = true;
    public void SetCorutineTimer(string zID, float t, Action<string> a)
    {
        if (corutineTimer)
        {
            StartCoroutine(CorutineTimer(zID, t, a));
        }
    }
    public void StopCorutineTimer(string zID, float t, Action<string> a)
    {
        corutineTimer = false;
        StopCoroutine(CorutineTimer(zID, t, a));
    }
    protected IEnumerator CorutineTimer(string zID, float t, Action<string> a)
    {
        yield return new WaitForSeconds(t);
        a(zID);
        SetCorutineTimer(zID, t, a);
    }
}