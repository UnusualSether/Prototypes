using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BattleStarter : MonoBehaviour
{
    public CharacterSpawner spawner;

    public GameObject emptyCharacter;

    public GameObject emptyPlayerCard;

    public CharacterData characterToSpawn;

    public List<GameObject> foundSpawnPoints;

    public List<GameObject> emptyCharacters;

    public List<CharacterData> charactersToSpawnData;

    public List<CharacterInstance> charactersToSpawnInstances;

    [ContextMenu("Spawn Empties")]
    private void SpawnEmpties()
    {
        if (spawner == null) 
        {
            Debug.Log("Error! No spawner found.");
            return;
        }

        foundSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint").ToList();

        if (foundSpawnPoints.Count == 0)
        {
            Debug.Log("Error, no spawn points found.");
            return;
        }

        foreach (var spawnPoint in foundSpawnPoints)
        {
            emptyCharacters.Add(Instantiate(emptyCharacter, spawnPoint.transform.position, Quaternion.identity));
        }

        if (emptyCharacters.Count == 0)
        {
            Debug.Log("Error! Empties weren't correctly spawned.");
            return;
        }

        foundSpawnPoints.Clear();

    }
    [ContextMenu("Apply Character to Empties")]
    private void SpawnNewCharacters()
    {
        if (charactersToSpawnData.Count == 0)
        {
            Debug.Log("Data list is empty. Returning...");
            return;
        }

        foreach (var data in charactersToSpawnData)
        {
            charactersToSpawnInstances.Add(new CharacterInstance(data));
        }

       


        for (int i = 0; i < charactersToSpawnInstances.Count; i++)
        {

            emptyCharacters[i].GetComponent<CharacterView>().ApplyDataAndVisuals(charactersToSpawnInstances[i]);
            
            if (emptyCharacters[i].GetComponent<SpriteRenderer>().sprite == null)
            {
                emptyCharacters.Remove(emptyCharacters[i]);
            }
        }
        



    }
    
   
}
