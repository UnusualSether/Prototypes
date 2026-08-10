using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RandomEncounterData", menuName = "Encounter/RandomEncounterData")]
public class RandomEncounterData : EncounterData
{



    public override EnemyData ReturnTheEnemyType()
    {
        int randomIndex = Random.Range(0, enemies.Count);

        return enemies[randomIndex];
    }

}
