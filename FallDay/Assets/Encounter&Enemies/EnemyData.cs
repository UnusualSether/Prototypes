using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;

    [Multiline]
    public string devToDevDescription;

    public int HP;

    public int Damage;
    

    public float phaseTimer;

    public GameObject Zprefab;
}
