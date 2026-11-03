using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UIElements;
using static GameHandler;
using static GameHandler.Encounter;


[Serializable]
public class Zombie
{
    public int id = 0;

    //HP and phase timer are not set by the gamehandler, but by the EnemyData which dictates the enemy's type.
    public int hp;
    public float PhaseTimer;
    public static int DifficultyValue = 0;
    public double Diff = 0;
    public string EnemyType;

    public Enemy3dBehaviour zombie_3d_space;

    private float PhT1 = 0;
    private bool IsFirstUpdate = true;
    public Difficulty difficulty;
    public int damage;
    public bool debugisOn = false;
    public GameHandler handler; //Isso é para a outra parte do codigo que foi movido para o game handler

    public List<ZombieEffect> effects = new List<ZombieEffect>();
            
    public EnemyData enemyData {  get; private set; }

    //public EnemyData enemyData; // BackUp em caso de falta de Informação.
    public enum ZombiePhase
    {
        Stop,
        Far,
        Approach,
        Close
    }

    public ZombiePhase phase;

    public int currentDisplay;


    public void GainEffect(ZombieEffect effect)
    {
        effects.Add(effect);

        effect.SetAttachedZombie(this);
    }

    public void UpdatePhase(float deltaTime)
    {
        if (TutorialControler.TutorialEnded == false)
        {
            phase = ZombiePhase.Stop;
        }

        if (phase == ZombiePhase.Close && PhT1 <= 0)
        {
            PlayerTookDamage?.Invoke(damage);
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
            //ChangePhase();
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
            if (debugisOn) Debug.Log($"Zombie with id {id} has changed phase to {phase}");
            handler.InvokePhaseChange(this);
        }
        else if (phase == ZombiePhase.Approach)
        {
            phase = ZombiePhase.Close;
            if (debugisOn) Debug.Log($"Zombie with id {id} has changed phase to {phase}");
            handler.InvokePhaseChange(this);
        }
    }

    public void Difficulty()
    {
        if (DifficultyValue == 1)
        {
            Diff = 0.5f;
            if (debugisOn) Debug.Log("Dificuldade virou Facil");
        }

        if (DifficultyValue == 2)
        {
            Diff = 1.0f;
            if (debugisOn) Debug.Log("Dificuldade virou Medio");
        }

        if (DifficultyValue == 3)
        {
            Diff = 2.0f;
            if (debugisOn) Debug.Log("Dificuldade virou Dificil");
        }
    }

    public void Die()
    {
        handler._KillZombie(id);
    }

    public void TakeDamage(int amount, out int total_damage)
    {
        RunOnTakeDamageEvents(amount, out bool take_damage_or_not, out int filtered_effect_damage);

        if (take_damage_or_not == false)
        {
            total_damage = amount;

            return;
        }


        total_damage = filtered_effect_damage;

        hp -= filtered_effect_damage;
    }

   public void Heal(int amount)
    {
        hp += amount;
    }

   public void CheckForOnDealDamageEffects()
    {
        foreach (IOnDealDamage deal_damage_effects in effects)
        {
            deal_damage_effects.DealDamage();
        }
    }

    public void Pushback()
    {
        zombie_3d_space.Pushback3DSpace();
    }
    public void DealDamage()
    {
        RunOnDealDamageEvents();


        handler.CallPlayerDamage(damage);
    }

    public void RunOnDealDamageEvents()
    {
        foreach (var on_deal_damage_effects in effects.OfType<IOnDealDamage>())
        {
            on_deal_damage_effects.DealDamage();
        }
    }

    public void RunOnTakeDamageEvents(int amount, out bool damage_possible, out int effect_filtered_damage)
     {
        bool can_take_damage = true;

        effect_filtered_damage = amount;


        foreach (var take_damage_effects in effects.OfType<IOnTakeDamage>())
        {
            take_damage_effects.TakeDamage(amount, out bool should_zombie_take_damage, out int effect_on_damage);

            effect_filtered_damage += effect_on_damage;
            

            if (!should_zombie_take_damage)
            {
                can_take_damage = false;
            }
        }

        if (!can_take_damage)
        {
            damage_possible = false;
        }

        else
        {
            damage_possible = true;
        }
    }

   

    public Zombie(EnemyData data)
    {
        enemyData = data;

        hp = (int)(data.HP);
        PhaseTimer = data.DamegeTimer;
        damage = data.Damage;
        EnemyType = data.enemyName;
    }
}
