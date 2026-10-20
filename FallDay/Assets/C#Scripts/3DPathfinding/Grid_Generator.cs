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
    public GameHandler gameHandler; // Reference to the GameHandler script, which manages game events and state
    public GameObject player; // Reference to the player GameObject, used for positioning and interactions
    
    private RoomIndexer roomIndexer; // Reference to the RoomIndexer script, which holds references to objects in the room for easy access
    private Transform[] roomIndexerAlternative; // Alternative array of Transforms for spawn positions, if RoomIndexer is not used (startingRoom)
    private bool useAlternativeRoomIndexer = false; // Flag to determine whether to use the alternative room indexer or the RoomIndexer script
    /// 3D Enemys Refrence
    private Dictionary<Zombie, GameObject> ThreeD_Zombie = new();//Stores pairs of Zombie data and their corresponding 3D game objects for easy access and management>
    
    ////////////////////////////////////
    /**/ public bool debug = true; /**/         // Debugging flag to enable or disable debug logs & other debug features
    ////////////////////////////////////




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




    //////////////////////////////////////////////////////////////////////
    // ----------------- Grid methods --------------------------------- //
    //////////////////////////////////////////////////////////////////////
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

    /// To Get the spawn position for the zombie, based on the grid and the player position. This method is called when the player enters a new room.
    int c = 0; // Variable to keep track of the current spawn position index for cycling through spawn positions
    public void setRoomIndexer(Transform[] roomIndexerAlternative) // Set the alternative room indexer reference for this Grid_Generator
    {
        this.roomIndexerAlternative = roomIndexerAlternative;
        c = 0;
        useAlternativeRoomIndexer = true; // Set the flag to use the alternative room indexer
    }
    public void setRoomIndexer(RoomIndexer roomIndexer) // Set the RoomIndexer reference for this Grid_Generator
    {
        this.roomIndexer = roomIndexer;
        c = 0;
        useAlternativeRoomIndexer = false; // Set the flag to use the RoomIndexer script
    }
    ///
    
    public Vector3 getSpawnPosition() // Called when player Enters a new room, to get a spawn position for the zombie, based on the grid and the player position.
    {
        if (useAlternativeRoomIndexer)
        {
            if (roomIndexerAlternative == null || roomIndexerAlternative.Length == 0)
            {
                Debug.LogError("roomIndexerAlternative in Grid_Generator is not set or empty.");
                return Vector3.zero; // Return a default value if the alternative room indexer is not set
            }
            Vector3 spawnPosition = roomIndexerAlternative[c].position;
            c = (c + 1) % roomIndexerAlternative.Length; // Cycle through spawn positions
            return spawnPosition;
        }
        else
        {
            if (roomIndexer == null || roomIndexer.zombiesSpawnPositions == null || roomIndexer.zombiesSpawnPositions.Length == 0)
            {
                Debug.LogError("RoomIndexer or zombiesSpawnPositions in Grid_Generator is not set or empty.");
                return Vector3.zero; // Return a default value if the RoomIndexer or spawn positions are not set
            }
            Vector3 spawnPosition = roomIndexer.zombiesSpawnPositions[c].transform.position;
            c = (c + 1) % roomIndexer.zombiesSpawnPositions.Length; // Cycle through spawn positions
            return spawnPosition;
        }
    }

    public void generateEnemy(Zombie zombie)
    {
        Vector3 spawnPosition = getSpawnPosition();
        grid.GetXYZ(spawnPosition, out int x, out int y, out int z);
        spawnPosition = grid.CellWorldPosition(x, 0, z);
        generateEnemy(zombie, spawnPosition);
    }
    private void generateEnemy(Zombie zombie, Vector3 spawnPosition)
    {
        GameObject spawnedEnemy;
        spawnedEnemy = Instantiate(zombie.enemyData.Zprefab, spawnPosition, Quaternion.identity);

        Enemy3dBehaviour Enemy3dBehaviour;
        PathFolower pathFolowerComponer;

        Enemy3dBehaviour = spawnedEnemy.GetComponent<Enemy3dBehaviour>();
        pathFolowerComponer = spawnedEnemy.GetComponent<PathFolower>();
        ThreeD_Zombie.Add(zombie, spawnedEnemy);
        
        Enemy3dBehaviour.Zombie3SetUP(zombie, gameHandler, player, grid, pathFolowerComponer);
        Enemy3dBehaviour.startWalk();
    }


    public void zombieDeath(Zombie zombie)
    {
        ThreeD_Zombie[zombie].GetComponent<Enemy3dBehaviour>().callerDestroy(zombie);
        ThreeD_Zombie.Remove(zombie);
    }




    /////////////////////////////////////////////////////////////
    // ----------------- GridDebugFuiinction ----------------- //
    /////////////////////////////////////////////////////////////
    int x = 0; // Variables to keep track of the current cell coordinates for cycling through spawn positions
    int z = 0;
    int changeCase = 0; 
    public Vector3 cicleThroghSpawnPos() // This method is for testing purposes (possebly to the game), to cycle through spawn positions and log them // || GridDebugFuiinction || \\
    {
        if (grid != null && grid.CellWorldPosition(x, 0, z) != null)
        {
            grid.returnWidthLenghHeight(out int width, out int hight, out int lengh);
            Vector3 worldPos = grid.CellWorldPosition(x, 0, z);
            Debug.Log($"Cell ({x}, {z}) World Position: {worldPos}");
            switch (changeCase) {
                case 0:
                    x = x + 3;
                    if (x >= width) {
                        changeCase = 1;
                        x = 0; // Reset x to 0 when switching to the next case
                    }
                    break;
                case 1:
                    z = z + 3;
                    changeCase = 0;
                    if(z >= lengh) {
                        z = 0; // Reset z to 0 when it exceeds the length
                    }

                    break;
            }
            return worldPos; // Return the first position for testing; you can modify this to return other positions as needed
        }
        Debug.LogError($"Grid is null or empty. Cannot cycle through spawn positions.");
        return Vector3.zero; // Return a default value if the grid is null or empty
    }
}
