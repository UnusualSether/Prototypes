using System.Collections.Generic;
using UnityEngine;

public class RoomCode
{
    private string Name;
    private int EnemyCount;
    private Transform SpawnPoint;
    public RoomCode(string Name, int EnemyCount, Transform SpawnPoint)
    {
        this.Name = Name;
        this.EnemyCount = EnemyCount;
        this.SpawnPoint = SpawnPoint;
    }
    public int ReturnEnemysCount()
    {
        return EnemyCount;
    }
    public Transform ReturnSpawn()
    {
        return SpawnPoint;
    }
}
public class RoomManeger
{
    private Dictionary<string, RoomCode> Rooms = new Dictionary<string, RoomCode>();

    public void AddRoom(string Name, int EnemyCount, Transform SpawnPoint)
    {
        RoomCode room = new RoomCode(Name, EnemyCount, SpawnPoint);
        Rooms.Add(Name, room);
        Debug.Log("Room " + Name + " added with " + EnemyCount + " enemies.");
    }
    public void RemoveRoom(string Name)
    {
        Rooms.Remove(Name);
        Debug.Log("Room " + Name + " removed.");
    }
    public int GetEnemysNum(string Name)
    {
        Debug.Log("Getting Room " + Name);
        RoomCode room = Rooms[Name];
        Debug.Log("Room Enemys = " + room.ReturnEnemysCount());
        return room.ReturnEnemysCount();
    }
    public Transform GetSpawnPoint(string Name)
    {
        RoomCode a = Rooms[Name];
        Debug.Log("Transform is this" + a.ReturnSpawn());
        return a.ReturnSpawn();
    }
}
