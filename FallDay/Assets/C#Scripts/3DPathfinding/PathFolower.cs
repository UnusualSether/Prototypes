using System;
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
    private bool isThereTarget = false; // Control say there is a path terget start pos is always this game object
    private bool errorThrow = false; // Exists to loop a new path Once before throwing an error
    private List<Vector3> path; 
    public Grid_ grid;
    private Action AfterWalkCall; //Used to call a function afer walk is complete

    /////////////////////////////////////
    /**/ private bool debug = false; /**/  //Debugging flag to enable or disable debug logs and other debug features
    /////////////////////////////////////
    
    public void pathFolowerSetUp(Grid_ grid, Vector3 targetPosition, int ReducePathOveride, Action afterWalkCall)
    {
        this.grid = grid;
        pathFolowerSetUp(targetPosition, ReducePathOveride, afterWalkCall);
    }
    public void pathFolowerSetUp(Vector3 targetPosition, int ReducePathOveride, Action afterWalkCall)
    {
        //set up 
        this.targetPosition = targetPosition;
        isThereTarget = true;
        AfterWalkCall = afterWalkCall;
        path = grid.pathfinding.FindPath(transform.position, new Vector3(this.targetPosition.x, grid.CellWorldPosition(0, 0, 0).y, this.targetPosition.z)); // 
        path.RemoveAt(path.Count - ReducePathOveride);
    }
    public void SerchNewPath() // Recheck the pathfinding in case the object is not close enough to the start position of the path (attempts to save in case the path becomes invalid) {Reroll}
    {
        if (isThereTarget) { 
        path = grid.pathfinding.FindPath(transform.position, new Vector3(this.targetPosition.x, grid.CellWorldPosition(0, 0, 0).y, this.targetPosition.z)); // 
        path.RemoveAt(path.Count - 1);
        if (debug) Debug.Log($"Path found with {path.Count} points");
        }
    }
    private IEnumerator WalkPathCoroutine()
    {
        if (isThereTarget) // Check if there is a target position to move towards (avoids null reference)
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
        if (isThereTarget)
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
                        errorThrow = true;
                        if (debug) Debug.Log("Starting Walk Coroutine");
                        StartCoroutine(WalkPathCoroutine());
                    }
                    else if ((gameObject.transform.position - path[0]).magnitude < 0.2f)
                    {
                        //Call a new pathfinding
                        if (errorThrow == false)
                        {
                            Debug.LogWarning("Not close enough to start pos, need to find new path");
                            errorThrow = true;
                            SerchNewPath();
                        }
                        else
                        {
                            Debug.LogError("Unable to find a valid path, stopping pathfinding");
                        }
                    }
                    if ((gameObject.transform.position - path[path.Count - 1]).magnitude < 0.2f)
                    {
                        Debug.Log($"Has gotten Close to the end of the path");
                        AfeterpathCall();
                    }
                }
            }
        }
    }
    private void AfeterpathCall()
    {
        if(AfterWalkCall != null)
        {
            AfterWalkCall();
        }
    }
}