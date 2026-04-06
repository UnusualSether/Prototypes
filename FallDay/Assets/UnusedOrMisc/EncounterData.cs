using UnityEngine;

[CreateAssetMenu(fileName = "EncounterData", menuName = "Scriptable Objects/EncounterData")]
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



}
