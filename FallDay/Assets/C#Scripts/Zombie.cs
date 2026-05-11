using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using static GameHandler;
using System;

[Serializable]
public class Zombie
{
    public int id = 0;

    //HP and phase timer are not set by the gamehandler, but by the EnemyData which dictates the enemy's type.
    public int hp;
    public float PhaseTimer;

    private float PhT1 = 0;
    private bool IsFirstUpdate = true;
    public GameHandler handler;
    //
    public enum ZombiePhase
    {
        Stop,
        Far,
        Approach,
        Close
    }

    public ZombiePhase phase;

    public int currentDisplay;

    //public static event Action<Zombie> ZombieIsClose;

    public void UpdatePhase(float deltaTime)
    {
        if (TutorialControler.TutorialEnded == false)
        {
            phase = ZombiePhase.Stop;
        }

        if (phase == ZombiePhase.Close && PhT1 <= 0)
        {
            PlayerTookDamage?.Invoke(1f);
            // <= Place a Destroy Zombie Call

            destroyZombie?.Invoke(this);
        }
        else if (IsFirstUpdate)
        {
            IsFirstUpdate = false;
            PhT1 = PhaseTimer;
        }
        else if (PhT1 <= 0)
        {
            PhT1 = PhaseTimer;
            ChangePhase();
        }
        else
        {
            PhT1 -= deltaTime;
        }
    }
    private void ChangePhase()
    {
        if (phase == ZombiePhase.Far)
        {
            phase = ZombiePhase.Approach;
            Debug.Log($"Zombie with id {id} has changed phase to {phase}");
            handler.InvokePhaseChange(this);
        }
        else if (phase == ZombiePhase.Approach)
        {
            phase = ZombiePhase.Close;
            Debug.Log($"Zombie with id {id} has changed phase to {phase}");
            handler.InvokePhaseChange(this);
            handler.InvokeZombieIsClose(this);
        }
    }

    public Zombie(EnemyData data)
    {
        hp = data.HP;
        PhaseTimer = data.phaseTimer;
    }
}
