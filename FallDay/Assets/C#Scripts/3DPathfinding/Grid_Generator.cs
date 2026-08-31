using UnityEngine;


// <summary>
/// <summary>
/// Add this script to a Player GameObject to generate a grid of cells based on the specified grid size. Around the player position. 
/// The grid size can be set in the inspector, and the grid will be generated when the game starts. 
/// </summary>


//[ExecuteInEditMode]
public class Grid_Generator : MonoBehaviour
{
    public Vector3 gridSise;
    public bool debug;
    private Grid_ grid;
    void Awake()
    {
        if (gridSise == null) 
        { 
            gridSise = new Vector3(10, 1, 10); 
            if(debug) Debug.Log("Grid Size NotSetInpo: " + gridSise);
        }
    }

    void Start()
    {
        Vector3 gridStartPos = this.gameObject.transform.position - new Vector3(gridSise.x / 2, gridSise.y, gridSise.z / 2); // Set the grid centered around the player position
        grid = new Grid_(gridSise, 1f, gridStartPos, this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
