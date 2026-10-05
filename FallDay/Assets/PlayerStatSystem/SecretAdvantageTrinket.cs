using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSecretAdvantageTrinket", menuName = "Trinket/Secret Advantage Trinket")]
[Serializable]
public class SecretAdvantageTrinket : Trinket, IDamageFilterTrinket, IPullInfoFromEncounterTrinket
{

    public string AdvantageousEnemy;

    public string DisadvantegousEnemy;

    public int reduced_damage;

    public int amplified_damage;



    public void PullEncounterInfo(EncounterData encounter_data)
    {
        if (encounter_data.enemies.Count == 1)
        {
            Debug.Log($"{trinket_name} Only one enemy. Backing out.");

            return;
        }

        

        AdvantageousEnemy = encounter_data.RandomEnemy().enemyName;

        DisadvantegousEnemy = encounter_data.RandomEnemy().enemyName;

        if (DisadvantegousEnemy == AdvantageousEnemy)
        {
            PullEncounterInfo(encounter_data);
            return;
        }

        Debug.Log($"{trinket_name} Advantage is {AdvantageousEnemy} Disadvantage is {DisadvantegousEnemy}");
    }
    public int ModifiedDamage(int damage, Zombie target)
    {
        if (target.EnemyType == AdvantageousEnemy)
        {
            return damage + amplified_damage;
        }

        if (target.EnemyType == DisadvantegousEnemy)
        {
            return damage - reduced_damage;
        }

        else
        {
            return damage;
        }

    }


}
