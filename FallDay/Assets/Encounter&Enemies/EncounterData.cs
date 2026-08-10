using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EncounterData", menuName = "Encounter/EncounterData")]
public class EncounterData : ScriptableObject
{
    [Header("DEV ONLY ENCOUNTER DESCRIPTION")]
    [Space(3)]
    [Header("Explain the encounters main purpose, its enemies and what you're aiming for")]
    [Header("in this encounter. ")]
    [SerializeField, MultilineAttribute]
    string devToDevDescription;


    [Header("ENCOUNTER PARAMETERS")]


    public GameHandler.Encounter.Difficulty difficulty;

    public int numberOfZombies;

    public float zombieTimer;

    public List<EnemyData> enemies;

    public virtual void SetParameters()
    {

    }

    public virtual EnemyData ReturnTheEnemyType()
    {
        return enemies[0];
    }



}
