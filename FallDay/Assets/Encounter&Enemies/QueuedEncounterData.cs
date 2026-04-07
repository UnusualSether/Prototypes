using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "QueuedEncounterData", menuName = "Encounter/QueuedEncounterData")]
public class QueuedEncounterData : ScriptableObject
{


    [Description("Add the enemies you want to show up in this list, remembering that their position in the list will be their position in the queue.")]
    [SerializeField]
    List<EnemyData> enemiesToQueue;


    public Queue<EnemyData> spawnQueue;



}
