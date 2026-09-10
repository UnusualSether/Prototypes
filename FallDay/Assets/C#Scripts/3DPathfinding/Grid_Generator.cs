using System.Collections.Generic;
using UnityEngine;


// <summary>
/// <summary>
/// Add this script to a Player GameObject to generate a grid of cells based on the specified grid size. Around the player position. 
/// The grid size can be set in the inspector, and the grid will be generated when the game starts. 
/// </summary>


//[ExecuteInEditMode]
public class Grid_Generator : MonoBehaviour
{
    //grid information
    public Vector3 gridSize;  // The size of the grid in terms of number of cells in each dimension (x, y, z)
    public Vector3 gridCenterOffset; // The offset to apply to the grid's center position relative to the GameObject's position
    public float cellSize = 1f; // The size of each cell in the grid
    public float cellHeightOverrite = 0f;
    private Grid_ grid;

    // References for other components and objects
    public GameHandler gameHandler;
    public GameObject player;

    /// Enemys Refrence
    private Dictionary<Zombie, GameObject> ThreeD_Zombie = new();//Stores pairs of Zombie data and their corresponding 3D game objects for easy access and management>

    public bool debug; // Debugging flag to enable or disable debug logs

    void Awake()
    {
        if (gridSize == null) 
        { 
            gridSize = new Vector3(10, 1, 10); 
            if(debug) Debug.Log("Grid Size NotSetInpo: " + gridSize);
        }
        gameHandler.ZombieSpawned += generateEnemy;
        gameHandler.ZombieKilled += zombieDeath;


        // Genetates Grid with the specified size and cell size, centered around the GameObject's position with an optional offset
        Vector3 posCorrection = new Vector3(-(cellSize * gridSize.x) / 2, -(cellSize * gridSize.y) / 2, -(cellSize * gridSize.z) / 2);
        Vector3 gridStartPos = this.gameObject.transform.position + posCorrection + gridCenterOffset; // Set the grid centered around the player position
        grid = new Grid_(gridSize, cellSize, cellHeightOverrite, gridStartPos, this.gameObject);
        grid.checkWalkableAll();
    }
    private void OnDisable()
    {
        gameHandler.ZombieSpawned -= generateEnemy;
    }
    void Start()
    {

    }
    private void Update()
    {
        grid.DisplayGridOutline();
    }
    public Grid_ GetGrid()
    {
        return grid;
    }
    // Manage The 3DZombie In world
    public void generateEnemy(Zombie zombie) //Maybe this should be in a different script, but for now it is here
    {
        //Zombies3D.Add(Instantiate(zombie.enemyData.Zprefab, grid.CellWorldPosition((int)(gridSize.x / 2), (int)gridSize.y -1, (int)(gridSize.z) - 1), Quaternion.identity));

        ThreeD_Zombie.Add(zombie, Instantiate(zombie.enemyData.Zprefab, grid.CellWorldPosition((int)(gridSize.x / 2), (int)gridSize.y - 1, (int)(gridSize.z) - 1), Quaternion.identity));

        int i = ThreeD_Zombie.Count - 1;
        ThreeD_Zombie[zombie].GetComponent<PathFolower>().Zombie3dInfoReceve(zombie, grid, player); // Pass Refrence
        ThreeD_Zombie[zombie].GetComponent<PathFolower>().canWalk(); // Start Walk Coroutine
    }
    public void zombieDeath(Zombie zombie)
    {
        ThreeD_Zombie[zombie].GetComponent<PathFolower>().callerDestroy(zombie);
        ThreeD_Zombie.Remove(zombie);
    }
}
