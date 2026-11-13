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

    public float speed = 0.0001f; //Speed of the object along the path
    public float rotationSpeed = 0.5f; // Speed of rotation when turning towards the next point in the path

    private Vector3 targetPosition = Vector3.negativeInfinity; // Target position to move towards

    [HideInInspector]
    public bool IsWalking { get; private set; } = false; // Flag to indicate if the object is currently walking along the path

    private bool isThereTarget = false; // Control say there is a path terget start pos is always this game object
    private List<Vector3> path;
    public Grid_ grid;
    private Action AfterWalkCall; //Used to call a function afer walk is complete
    private int ReducePathOveride;
    /////////////////////////////
    private bool debug = true; // Debugging flag to enable or disable debug logs and other debug features
    /////////////////////////////
    public void pathFolowerSetUp(Grid_Generator gridGen, Vector3 targetPosition, int ReducePathOveride, Action afterWalkCall)
    {
        this.grid = gridGen.grid;
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
        else {Debug.LogError($"[{gameObject.name}]Folower SetUp Incomplete"); }
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
        else { Debug.LogError($"[{gameObject.name}]Folower SetUp Incomplete"); }
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
            if (debug) Debug.Log($"[{gameObject.name}]Path found with {path.Count} points");
        }
    }
    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    // Emrgency check to see if the object is close enough to the start position of the path,
    // and if not, it will attempt to find a new path.
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
                    if ((gameObject.transform.position - path[path.Count - 1]).magnitude < 0.2f) // Has Reached Target
                    {
                        if(debug) {Debug.Log($"[{gameObject.name}]Has gotten Close to the end of the path");}
                        AfeterpathCall();
                        return;
                    }

                    Vector3 a = new Vector3 (gameObject.transform.position.x, 0, gameObject.transform.position.z); // Pos atual
                    Vector3 b = new Vector3(path[0].x, 0, path[0].z); //Pos inicial do caminho
                    if ((a - b).magnitude < 0.3f) // is close enough to start pos (is path valid)
                    {
                        //is close enough to stert pos
                        //start walk
                        if (debug) {Debug.Log($"[{gameObject.name}]Starting Walk Coroutine if is {(a - b).magnitude < 0.3f}");}
                        WalkPath();
                    }
                    else
                    {
                        if (debug) {Debug.LogWarning($"[{gameObject.name}]To far to start Walk");}
                    }
                }
            }
        }
    }

    public void aimIntoPosition(Vector3 target, Action endCall)
    {
        StartCoroutine(TurnToTargetCoroutine(target, endCall));
    }


    private void WalkPath(int index = 0)
    {
        if (isThereTarget && targetPosition != Vector3.negativeInfinity) // Check if there is a target position to move towards (avoids null reference)
        {
            if (index < path.Count)
            {
                Vector3 nextPoint = path[index];
                StartCoroutine(TurnToTargetCoroutine(nextPoint, () =>
                {
                    StartCoroutine(WalkToNextPoint(nextPoint, () =>
                    {
                        WalkPath(index + 1); // Recursively call WalkPath with the next index
                    }));
                }));
            }
            else
            {
                AfeterpathCall(); // Call the AfterWalkCall action when the path is completed
            }
        }
    }

    private IEnumerator WalkToNextPoint(Vector3 nextPoint, Action endCall)
    {
        IsWalking = true;

        // 1. A 'while' loop is safer than 'do-while' in case the object is already at the destination.
        // 2. Vector3.MoveTowards automatically clamps at the exact target without overshooting, 
        // so checking for exact inequality is completely safe and avoids calculating magnitudes.
        while (transform.position != nextPoint)
        {
            // 3. Match Time.deltaTime with yield return null (Update loop)
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, nextPoint, step);

            yield return null;
        }

        IsWalking = false;

        // 4. Simplify the callback invocation
        endCall?.Invoke();
    }
    private IEnumerator TurnToTargetCoroutine(Vector3 target, Action endCall)
    {
        // 1. Flatten the target position to the object's Y level before calculating direction.
        // This safely prevents tilting without breaking quaternion mathematics.
        Vector3 flatTarget = new Vector3(target.x, transform.position.y, target.z);
        Vector3 directionToTarget = (flatTarget - transform.position).normalized;

        // Prevent LookRotation from throwing an error if the object is exactly on the target
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // 2. Use Quaternion.Angle for a predictable threshold (e.g., stop when within 1 degree)
            while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
            {
                // 3. Match Time.deltaTime with yield return null (Update loop)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                yield return null;
            }

            // Snap to the exact target rotation to prevent leftover micro-inaccuracies
            transform.rotation = targetRotation;
        }

        // 4. Simplify the callback with the null-conditional operator
        endCall?.Invoke();
    }
    private void AfeterpathCall()
    {
        if (AfterWalkCall != null)
        {
            AfterWalkCall();
        }
    }
}