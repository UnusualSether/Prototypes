using System;
using System.Collections;
using UnityEngine;

// Class for Monster/Zomby Behavior
public class ZombieLogic
{
    public bool corutineTimer = true;
    public void SetCorutineTimer(int zID, float t, Action a)
    {
        if (corutineTimer)
        {
            CorutineTimer(zID, t, a);
        }
    }
    public void StopCorutineTimer(int zID, float t, Action a)
    {
        corutineTimer = false;
    }
    public IEnumerator CorutineTimer(int zID, float t, Action a)
    {
        yield return new WaitForSeconds(t);
        if (corutineTimer)
        {
            a();
            SetCorutineTimer(zID, t, a);
        }
    }
}