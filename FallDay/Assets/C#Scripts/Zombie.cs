using UnityEngine;
using static GameHandler;

public class Zombie
{
    public int id = 0;
    public int hp;
    public float PhaseTimer = 5;
    private float PhT1 = 0;
    private bool IsFirstUpdate = true;

    public enum ZombiePhase
    {
        Stop,
        Far,
        Approach,
        Close
    }

    public ZombiePhase phase;

    public int currentDisplay;

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

            destroyZombie?.Invoke(id);
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
        }
        else if (phase == ZombiePhase.Approach)
        {
            phase = ZombiePhase.Close;
            Debug.Log($"Zombie with id {id} has changed phase to {phase}");
        }
    }
}
