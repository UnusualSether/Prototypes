using UnityEngine;


// <summary>
/// <summary>
/// Add this script to a Player GameObject to generate a grid of cells based on the specified grid size. Around the player position. 
/// The grid size can be set in the inspector, and the grid will be generated when the game starts. 
/// </summary>


//[ExecuteInEditMode]
public class Grid_Generator : MonoBehaviour
{
    public Vector3 gridSize;
    public Vector3 gridCenterOffset;
    public float cellSize = 1f;
    public float cellHeightOverrite = 0f;
    public GameHandler gameHandler;
    public bool debug;
    public bool checkForObstacle = false;
    private Grid_ grid;
    void Awake()
    {
        if (gridSize == null) 
        { 
            gridSize = new Vector3(10, 1, 10); 
            if(debug) Debug.Log("Grid Size NotSetInpo: " + gridSize);
        }
        gameHandler.ZombieSpawned += generateEnemy;
    }

    void Start()
    {
        Vector3 posCorrection = new Vector3(-(cellSize * gridSize.x) / 2, -(cellSize * gridSize.y) /2, -(cellSize * gridSize.z) / 2);
        Vector3 gridStartPos = this.gameObject.transform.position + posCorrection + gridCenterOffset; // Set the grid centered around the player position
        grid = new Grid_(gridSize, cellSize, cellHeightOverrite, gridStartPos, this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        grid.checkWalkableAll();
        //listenToBool();
    }

    private void listenToBool()
    {
        if (checkForObstacle)
        {
            checkForObstacle = false;
            grid.checkWalkableAll();
        }
    }
    // Generate The Zombie In world
    public void generateEnemy(Zombie zombie) //Maybe this should be in a different script, but for now it is here
    {
        Instantiate(zombie.enemyData.Zprefab, grid.GetWorldPosition((int)(gridSize.x /2), (int)gridSize.y, (int)(gridSize.z)), Quaternion.identity);
    }
}
