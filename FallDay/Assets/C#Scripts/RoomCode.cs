using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomCode
{
    public string Name;
    public int EnemyCount;
    public Transform SpawnPoint;
    public RoomCode(string Name, int EnemyCount, Transform SpawnPoint) //<= Isso serve para forçar a criação de um objeto RoomCode com os parametros necessários.
    {
        this.Name = Name;
        this.EnemyCount = EnemyCount;
        this.SpawnPoint = SpawnPoint;
    }
    // Conciderando que as variaveis são publicas, não é necessário criar getters, Devo Rmover eles ou deixar como estão?
    public int ReturnEnemysCount()
    {
        return EnemyCount;
    }
    public Transform ReturnSpawn()
    {
        return SpawnPoint;
    }
}

[Serializable]
public class RoomManeger
{
    private Dictionary<string, RoomCode> RoomLookup = new Dictionary<string, RoomCode>();
    public List<RoomCode> RoomsList = new List<RoomCode>();
    public void AddRoom(string Name, int EnemyCount, Transform SpawnPoint)
    {
        RoomCode room = new RoomCode(Name, EnemyCount, SpawnPoint);
        RoomLookup.Add(Name, room);
        RoomsList.Add(room);
        Debug.Log("Room " + Name + " added with " + EnemyCount + " enemies.");
    }
    public void RemoveRoom(string Name)
    {
        RoomLookup.Remove(Name);
        Debug.Log("Room " + Name + " removed.");
    }
    public int GetEnemysNum(string Name)
    {
        Debug.Log("Getting Room " + Name);
        RoomCode room = RoomLookup[Name];
        Debug.Log("Room Enemys = " + room.ReturnEnemysCount());
        return room.ReturnEnemysCount();
    }
    public Transform GetSpawnPoint(string Name)
    {
        RoomCode a = RoomLookup[Name];
        Debug.Log("Transform is this" + a.ReturnSpawn());
        return a.ReturnSpawn();
    }
}
