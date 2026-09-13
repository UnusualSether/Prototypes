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

    /// 3D Enemys Refrence
    private Dictionary<Zombie, GameObject> ThreeD_Zombie = new();//Stores pairs of Zombie data and their corresponding 3D game objects for easy access and management>

    ///////////////////////////////////////////////////////////////////////////
    public bool debug; // Debugging flag to enable or disable debug logs
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    /// ------------------ Unity Lifecycle Methods -----------------
    ///////////////////////////////////////////////////////////////////////////
    void Awake()// Used to initialize the grid and subscribe to events from the GameHandler
    {
        if (gridSize == Vector3.zero) 
        {  
            if(debug) Debug.LogError("Grid Size NotSetInpo: " + gridSize);
        }
        if (gameHandler != null)
        {
            gameHandler.ZombieSpawned += generateEnemy;
            gameHandler.ZombieKilled += zombieDeath;
        }
        else { 
            if(debug) Debug.LogError("GameHandler NotSetInpo: " + gameHandler);
        }

        Vector3 initialOrigin = GetCenteredOriginPosition();
        grid = new Grid_(gridSize, cellSize, cellHeightOverrite, initialOrigin, this.gameObject);
        UpdateGrid();
    }
    private void OnDisable() // To avoid memory leaks, unsubscribe from the event when the object is disabled or destroyed
    {
        gameHandler.ZombieSpawned -= generateEnemy;
        gameHandler.ZombieKilled -= zombieDeath;
    }
    /////////////////////////////////////////////////////////////////////
    // ----------------- Grid methods -----------------
    /////////////////////////////////////////////////////////////////////

    public void UpdateGrid() // Update the grid's walkable status for all cells
    {
        CenterGridOnSelf();
    }
    public void CenterGridOnSelf()
    {
        if (grid == null) return;

        Vector3 newOrigin = GetCenteredOriginPosition();
        grid.UpdateGrid(newOrigin);
    }
    public Vector3 GetCenteredOriginPosition()
    {
        // Metade do tamanho total do grid em X e Z usando apenas cellSize
        float halfWidth = ((gridSize.x * cellSize) * 0.5f);
        float halfDepth = ((gridSize.z * cellSize) * 0.5f);


        // Adiciona metade da extensão em X e Z para centralizar; mantém o Y no nível do objeto
        Vector3 offsetToOrigin = new Vector3(halfWidth, -0.1f, halfDepth);

        return transform.position - offsetToOrigin;
    }
    /////////////////////////////////////////////////////////////////////
    // ----------------- Manage The 3DZombie In world -----------------
    /////////////////////////////////////////////////////////////////////
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
