using System.Collections.Generic;
using UnityEngine;

/// Add this script to a Player GameObject to generate a grid of cells based on the specified grid size. Around the player position. 
/// The grid size can be set in the inspector, and the grid will be generated when the game starts. 

//[ExecuteInEditMode]
public class Grid_Generator : MonoBehaviour
{
    //grid information
    public Vector3 gridSize;  // The size of the grid in terms of number of cells in each dimension (x, y, z)
    public Vector3 gridCenterOffset; // The offset to apply to the grid's center position relative to the GameObject's position
    public float cellSize = 1f; // The size of each cell in the grid
    public float cellHeightOverrite = 0f;
    public Grid_ grid { get; private set; }

    // References for other components and objects
    public GameHandler gameHandler;
    public GameObject player;
    public PlayerPathControl playerPathControl;

    /// 3D Enemys Refrence
    private Dictionary<Zombie, GameObject> ThreeD_Zombie = new();//Stores pairs of Zombie data and their corresponding 3D game objects for easy access and management>
    private Dictionary<Zombie, Enemy3dBehaviour> path3DController = new();
    private Dictionary<Zombie, PathFolower> PathFolowerDictionary = new();
    ////////////////////////////
    /**/
    public bool debug; /**/         // Debugging flag to enable or disable debug logs
    ////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    /// ------------------ Unity Lifecycle Methods -----------------
    ///////////////////////////////////////////////////////////////////////////
    void Awake()// Used to initialize the grid and subscribe to events from the GameHandler
    {
        if (gridSize == Vector3.zero)
        {
            if (debug) Debug.LogError("Grid Size NotSetInpo: " + gridSize);
        }
        if (gameHandler != null)
        {
            gameHandler.ZombieSpawned += generateEnemy;
            gameHandler.ZombieKilled += zombieDeath;
        }
        else
        {
            if (debug) Debug.LogError("GameHandler NotSetInpo: " + gameHandler);
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
    //////////////////////////////////////////////////////
    // ----------------- Grid methods ----------------- //
    //////////////////////////////////////////////////////

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


    //////////////////////////////////////////////////////////////////////
    // ----------------- Manage The 3DZombie In world ----------------- //
    //////////////////////////////////////////////////////////////////////
    public void generateEnemy(Zombie zombie) //Maybe this should be in a different script, but for now it is here
    {
        GameObject spawnedEnemy;
        spawnedEnemy = Instantiate(zombie.enemyData.Zprefab, cicleThroghSpawnPos(), Quaternion.identity);

        Enemy3dBehaviour Enemy3dBehaviour;
        PathFolower pathFolowerComponer;

        Enemy3dBehaviour = spawnedEnemy.GetComponent<Enemy3dBehaviour>();
        pathFolowerComponer = spawnedEnemy.GetComponent<PathFolower>();
        ThreeD_Zombie.Add(zombie, spawnedEnemy);
        Enemy3dBehaviour.Zombie3SetUP(zombie, gameHandler, player, grid, pathFolowerComponer);

        Enemy3dBehaviour.startWalk();
    }

    int x = 0; // Variables to keep track of the current cell coordinates for cycling through spawn positions
    int z = 0;
    int changeCase = 0; 
    public Vector3 cicleThroghSpawnPos() // This method is for testing purposes (possebly to the game), to cycle through spawn positions and log them
    {
        if (grid != null && grid.CellWorldPosition(x, 0, z) != null)
        {
            grid.returnWidthLenghHeight(out int width, out int hight, out int lengh);
            Vector3 worldPos = grid.CellWorldPosition(x, 0, z);
            Debug.Log($"Cell ({x}, {z}) World Position: {worldPos}");
            switch (changeCase) {
                case 0:
                    z++;
                    if (z >= lengh) {
                        changeCase = 1;
                        z = 0; // Reset x to 0 when switching to the next case
                    }
                    break;
                case 1:
                    x++;
                    changeCase = 0;
                    if(x >= width) {
                        x = 0; // Reset z to 0 when it exceeds the length
                    }

                    break;
            }
            return worldPos; // Return the first position for testing; you can modify this to return other positions as needed
        }
        Debug.LogError($"Grid is null or empty. Cannot cycle through spawn positions.");
        return Vector3.zero; // Return a default value if the grid is null or empty
    }

    public void zombieDeath(Zombie zombie)
    {
        ThreeD_Zombie[zombie].GetComponent<Enemy3dBehaviour>().callerDestroy(zombie);
        ThreeD_Zombie.Remove(zombie);
    }
}
