using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VectorGraphics;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
    public int number_of_rooms_to_complete;


    public string level_scene;

    public Level next_level; 

    public List<Reward> possible_rewards;

    public DifficultyLevel difficulty;


    public List<RoomData> possible_rooms;

    public List<EncounterData> possible_encounters;





}
