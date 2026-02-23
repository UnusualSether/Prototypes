using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "Rooms/Room")]
public class RoomData : ScriptableObject
{
    public string RoomName;



    [Header("Visual Prefab")]
    public GameObject prefab;

    [Header("Rewards")]
    public List<RewardData> possibleRewards = new List<RewardData>();    
}
