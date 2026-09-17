using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFolower : MonoBehaviour
{
    // This script adds a pathfinding component to a 3D object, allowing it to follow a path towards a target position.
    // It uses a grid-based pathfinding system to find the optimal path and moves the object along that path at a specified speed.
    // Needs a additional script to handle the target position and pathfinding logic (controller that can vary in behavior)
        // this was decided to be a separate script to allow for more flexibility in the pathfinding logic and to avoid cluttering the PathFolower script with too much functionality.
    // needs a grid system to define the walkable areas in the game world.
    // Grid_ can be changed to any other grid system that implements the same pathfinding logic, as long as it provides a method to find a path between two positions.

    public float speed = 1.0f; //Speed of the object along the path
    private Vector3 targetPosition; // Target position to move towards
    private bool isThereTarget = false;
    private bool errorThrow = false;
    private List<Vector3> path;
    private Grid_ grid;
    ///////////////////////////////////
    /**/ private bool debug = false; // Debugging flag to enable or disable debug logs and other debug features
    ///////////////////////////////////
    
    public void pathFolowerSetUp(Grid_ grid, Vector3 targetPosition)
    {
        //set up 
        this.targetPosition = targetPosition;
        this.grid = grid;

        isThereTarget = true;

        path = grid.pathfinding.FindPath(transform.position, new Vector3(this.targetPosition.x, grid.CellWorldPosition(0, 0, 0).y, this.targetPosition.z)); // 
        path.RemoveAt(path.Count - 1);
    }
    private void SerchNewPath() // Recheck the pathfinding in case the object is not close enough to the start position of the path (attempts to save in case the path becomes invalid) {Reroll}
    {
        path = grid.pathfinding.FindPath(transform.position, new Vector3(this.targetPosition.x, grid.CellWorldPosition(0, 0, 0).y, this.targetPosition.z)); // 
        path.RemoveAt(path.Count - 1);
        if (debug) Debug.Log($"Path found with {path.Count} points");
    }
    private IEnumerator WalkPathCoroutine()
    {
        if (isThereTarget == true) // Check if there is a target position to move towards (avoids null reference)
        {
            foreach (Vector3 point in path)
            {
                do
                {
                    float step = speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, point, step);
                    yield return new WaitForFixedUpdate();
                } while ((gameObject.transform.position - point).magnitude > 0.02f);
            }
            canWalk();
        }
    }
    // Emrgency check to see if the object is close enough to the start position of the path,
    // and if not, it will attempt to find a new path.
    // This is a safety measure in case the path becomes invalid or the object is not close enough to the start position.
    public void canWalk() 
    {
        //is pos of curerent Object near start pos?
        //true - start path
        //false - serch new path
        if (path != null)
        {
            if (path.Count > 0) // is there path to follow
            {
                if ((gameObject.transform.position - path[0]).magnitude < 0.2f) // is close enough to start pos (is path valid)
                {
                    //is close enough to stert pos
                    //start walk
                    if (debug) Debug.Log("Starting Walk Coroutine");
                    StartCoroutine(WalkPathCoroutine());
                }
                else
                {
                    //Call a new pathfinding
                    if(errorThrow == false)
                    {
                        Debug.LogWarning("Not close enough to start pos, need to find new path");
                        errorThrow = true;
                        //reSerchNewPath();
                    }
                    else
                    {
                        Debug.LogError("Unable to find a valid path, stopping pathfinding");
                    }
                }
                if((gameObject.transform.position - path[path.Count - 1]).magnitude < 0.2f)
                {
                    Debug.Log($"Has gotten Close to the end of the path");
                    // Add a damage event to the player here
                }
            }
        }
    }

}


// This script is attached to the 3D zombie prefab and handles its behavior in the game world

// It receives information about the zombie and the player
// can destroy itself when the zombie is killed
// Controls the pathFinding of the 3D zombie in the game world
// 
public class Individual3dEnemyController : MonoBehaviour //!!!!!! PLACE THIS IN A NOTHER SCRIPT FILE, THIS IS A TEMPORARY PLACEHOLDER !!!!!!//
{
    public PathFolower pathFolower; // Reference to the PathFolower component attached to the 3D zombie prefab
    
    private Zombie zombie; // Zombie reference to subscride to the damege && death Event and ID refrence
    private GameObject player; // Reference to the player object
    private bool activationGate = false; // Activation gate to prevent null reference errors when the zombie is not yet properly initialized

    public void Zombie3dInfoReceive(Zombie zombie, GameObject player) // Needs to be called by the initial spawner to pass the zombie and player references to this script
    {
        this.zombie = zombie;
        this.player = player;
        if(pathFolower == null) pathFolower = GetComponent<PathFolower>(); //in case the pathFolower is not set in the inspector, it will try to get it from the same gameObject

        activationGate = true;
    }
    public void startWalk() // This method is called by the 3DzombieGenerator when the zombie is spawned and is ready to start moving towards the player
    {
        if (activationGate)
        {
            //pathFolower.SerchNewPath();
        }
    }
    public void callerDestroy(Zombie zombie) 
    {
        if (activationGate && this.zombie == zombie)
        {
            Destroy(this.gameObject);
        }
    }
}