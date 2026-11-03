using UnityEngine;

public class RoomIndexer : MonoBehaviour
{
    // Script That holds all the refrences In a room to be used by other scripts
    // Minemises calls to GameObject.Find() and other expensive calls by storing the refrences in one place

    // For now this is only for grid based objects, but can be expanded to include other types of objects in the future
    public GameObject[] zombiesSpawnPositions;
}
