using UnityEngine;

public class RoomCodeTester : MonoBehaviour
{
    private RoomManeger roomManager = new RoomManeger();
    public Transform secondTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roomManager.AddRoom("Room1", 5, this.transform);
        roomManager.AddRoom("Rooamdw", 921, secondTransform);
        int room = roomManager.GetEnemysNum("Room1");
        Transform spawnPoint = roomManager.GetSpawnPoint("Room1");
        Debug.Log("Room1" + "Enemys Num: " + room);
        Debug.Log("Room1" + "Spawn Point: " + spawnPoint.position);



        room = roomManager.GetEnemysNum("Rooamdw");
        spawnPoint = roomManager.GetSpawnPoint("Rooamdw");
        Debug.Log("Rooamdm" + "Enemys Num: " + room);
        Debug.Log("Rooamdm " + "Spawn Point: " + spawnPoint.position);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
