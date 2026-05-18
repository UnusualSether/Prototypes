using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using static GameHandler;
using static GameHandler.Encounter;
using System;

public class Zombie
{
    public int id = 0;

    //HP and phase timer are not set by the gamehandler, but by the EnemyData which dictates the enemy's type.
    public int hp;
    public float PhaseTimer;
    public static int DifficultyValue = 0;
    public double Diff = 0;

    private float PhT1 = 0;
    private bool IsFirstUpdate = true;
    public Difficulty difficulty;

    public GameHandler handler; //Isso é para a outra parte do codigo que foi movido para o game handler

    public enum ZombiePhase
    {
        Stop,
        Far,
        Approach,
        Close
    }

    public ZombiePhase phase;

    public int currentDisplay;

    public event Action<Zombie> ZombieIsClose;

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
            //ZombieIsClose?.Invoke(this);

        }
    }

    public void Difficulty()
    {
        if (DifficultyValue == 1)
        {
            Diff = 0.5f;
            Debug.Log("Dificuldade virou Facil");
        }

        if (DifficultyValue == 2)
        {
            Diff = 1.0f;
            Debug.Log("Dificuldade virou Medio");
        }

        if (DifficultyValue == 3)
        {
            Diff = 2.0f;
            Debug.Log("Dificuldade virou Dificil");
        }
    }

    public Zombie(EnemyData data)
    {
        hp = (int)(data.HP * Diff);
        PhaseTimer = data.phaseTimer;
    }

    /*public void ZombieDead()
    {

    }*/


}
