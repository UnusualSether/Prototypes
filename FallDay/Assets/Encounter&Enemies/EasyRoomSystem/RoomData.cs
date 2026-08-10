using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "Rooms/Room")]
public class RoomData : ScriptableObject
{
    public string RoomName;

    [Multiline]
    public string devToDevDescription;

    [Header("Visual Prefab")]
    public GameObject prefab;

}
