using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(fileName = "QueuedEncounterData", menuName = "Encounter/QueuedEncounterData")]
public class QueuedEncounterData : EncounterData
{



    public Queue<EnemyData> spawnQueue = new Queue<EnemyData>();

    public override void SetParameters()
    {
        numberOfZombies = enemies.Count;

         if(enemies.Count == 0)
        {
            return;
        }

        foreach(var enemy in enemies)
        {
            spawnQueue.Enqueue(enemy);
        }
    }

    public override EnemyData ReturnTheEnemyType()
    {
        return spawnQueue.Dequeue();

    }


}
