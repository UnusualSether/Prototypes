using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFolower : MonoBehaviour
{
    // This script adds a pathfinding component to a 3D object, allowing it to follow a path towards a target position.
    // It uses a grid-based pathfinding system to find the optimal path and moves the object along that path at a specified speed.
    // Needs a additional script to handle the target position and pathfinding logic (controller that can vary in behavior)
    // this was decided to be a separate script to allow for more flexibility in the pathfinding
    // logic and to avoid cluttering the PathFolower script with too much functionality.
    // needs a grid system to define the walkable areas in the game world.
    // Grid_ can be changed to any other grid system that implements the same pathfinding logic, as long as it provides a method to find a path between two positions.

    public float speed = 1.0f; //Speed of the object along the path
    public float velocidadeRotacao; // Rotation speed of the object when turning towards the next point in the path

    private Vector3 targetPosition; // Target position to move towards

    private bool isThereTarget = false; // Control say there is a path terget start pos is always this game object
    private bool errorThrow = false; // Exists to loop a new path Once before throwing an error
    private List<Vector3> path;
    public Grid_ grid;
    private Action AfterWalkCall; //Used to call a function afer walk is complete
    private int ReducePathOveride;
    /////////////////////////////
    private bool debug = false; // Debugging flag to enable or disable debug logs and other debug features
    /////////////////////////////
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
        this.ReducePathOveride = ReducePathOveride;
    }
    public void FindPath()
    {
        if (isThereTarget)
        {
            path = grid.pathfinding.FindPath(transform.position, new Vector3(this.targetPosition.x, grid.CellWorldPosition(0, 0, 0).y, this.targetPosition.z)); // 
            if (ReducePathOveride > 0)
            {
                path.RemoveAt(path.Count - ReducePathOveride);
            }
        }
        else {Debug.LogError("Folower SetUp Incomplete"); }
    }

    public void FindPath(Vector3 a, Vector3 b)
    {
        if (isThereTarget)
        {
            path = grid.pathfinding.FindPath(a, b); // 
            if (ReducePathOveride > 0)
            {
                path.RemoveAt(path.Count - ReducePathOveride);
            }
        }
        else { Debug.LogError("Folower SetUp Incomplete"); }
    }

    public void setNewTarget(Vector3 target)
    {
        this.targetPosition = target;
    }
    public void SerchNewPath() // Recheck the pathfinding in case the object is not close enough to the start position of the path (attempts to save in case the path becomes invalid) {Reroll}
    {
        if (isThereTarget && targetPosition != Vector3.negativeInfinity)
        {
            path = grid.pathfinding.FindPath(transform.position, new Vector3(this.targetPosition.x, grid.CellWorldPosition(0, 0, 0).y, this.targetPosition.z)); // 
            path.RemoveAt(path.Count - 1);
            if (debug) Debug.Log($"Path found with {path.Count} points");
        }
    }
    private bool CanTurn;
    private float alinhamento;
    private IEnumerator WalkPathCoroutine()
    {
        if (isThereTarget && targetPosition != Vector3.negativeInfinity) // Check if there is a target position to move towards (avoids null reference)
        {
            //foreach (Vector3 point in path)
            for (int i = 0; i < path.Count; i++)
            {
                Vector3 nextPoint = path[i];
                Vector3 preveosPoint = i > 0 ? path[i - 1] : path[i];
                Vector3 vectorBetweenPoints;

                if (nextPoint == preveosPoint)
                {
                    vectorBetweenPoints = Vector3.zero;
                    CanTurn = false;
                }
                else
                {
                    vectorBetweenPoints = (nextPoint - preveosPoint).normalized;
                    CanTurn = true;
                }
                alinhamento = Vector3.Dot(transform.forward.normalized, vectorBetweenPoints);
                while (((gameObject.transform.position - nextPoint).magnitude > 0.02f) && alinhamento <= 0.99f && CanTurn) //I need a Vector that points twords The front of the character to check if the character is facing the point, if not, it will rotate the character to face the point before moving towards it
                {
                    Quaternion rotacaoAlvo = Quaternion.LookRotation(new Vector3(nextPoint.x, transform.position.y, nextPoint.z) - transform.position); //the target rotation to face the next point in the path
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacaoAlvo, velocidadeRotacao * Time.deltaTime);
                    alinhamento = Vector3.Dot(transform.forward.normalized, vectorBetweenPoints);
                    yield return new WaitForFixedUpdate();
                }
                if(alinhamento > 0.99f)
                {
                    transform.rotation = Quaternion.LookRotation(new Vector3(nextPoint.x, transform.position.y, nextPoint.z) - transform.position);
                }

                do
                {
                    float step = speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, nextPoint, step);
                    yield return new WaitForFixedUpdate();
                } while ((gameObject.transform.position - nextPoint).magnitude > 0.02f); // is Z pos near the next point in the path, if not, it will keep moving towards it
            }
            canWalk();
        }
        yield return null;
    }

    // Emrgency check to see if the object is close enough to the start position of the path,
    // and if not, it will attempt to find a new path.
    // This is a safety measure in case the path becomes invalid or the object is not close enough to the start position.
    public void canWalk()
    {
        if (isThereTarget && targetPosition != Vector3.negativeInfinity)
        {
            //is pos of curerent Object near start pos?
            //true - start path
            //false - serch new path
            if (path != null)
            {
                if (path.Count > 0) // is there path to follow
                {
                    Vector3 a = new Vector3 (gameObject.transform.position.x, 0, gameObject.transform.position.z); // Pos atual
                    Vector3 b = new Vector3(path[0].x, 0, path[0].z); //Pos inicial do caminho

                    if ((a - b).magnitude < 1) // is close enough to start pos (is path valid)
                    {
                        //is close enough to stert pos
                        //start walk
                        errorThrow = true;
                        if (debug) {Debug.Log("Starting Walk Coroutine");}
                        StartCoroutine(WalkPathCoroutine());
                    }
                    else
                    {
                        if (debug) {Debug.Log("To far to start Walk");}
                    }
                    if ((gameObject.transform.position - path[path.Count - 1]).magnitude < 0.2f)
                    {
                        if(debug) {Debug.Log($"Has gotten Close to the end of the path");}
                        AfeterpathCall();
                    }
                }
            }
        }
    }
    private void AfeterpathCall()
    {
        if (AfterWalkCall != null)
        {
            AfterWalkCall();
        }
    }
}