using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFolower : MonoBehaviour
{
    public float speed = 1.0f; //Speed of the object along the path

    private Zombie zombie; // Zombie reference to subscride to the damege && death Event and ID refrence
    private List<Vector3> path;
    private Grid_ grid;
    private GameObject player; // Reference to the player object
    // Most Simple movement along the path
    // No Control over the speed nor how long it takes to reach the next point 
    private bool debug = false;
    public void Zombie3dInfoReceve(Zombie zombie, Grid_ grid, GameObject player)
    {
        this.zombie = zombie;
        this.grid = grid;
        this.player = player;
        SerchNewPath();
    }
    public void SerchNewPath()
    {
        path = grid.pathfinding.FindPath(transform.position, new Vector3(player.transform.position.x, grid.CellWorldPosition(0, 0, 0).y, player.transform.position.z)); // 
        path.RemoveAt(path.Count - 1); 
    }

    private IEnumerator WalkPathCoroutine()
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
    }
    public void canWalk()
    {
        //is pos of curerent Object near start pos?
        //true - start path
        //false - serch new path
        if (path != null)
        {
            if (path != null && path.Count > 0)
            {
                if ((gameObject.transform.position - path[0]).magnitude < 0.2f)
                {
                    //is close enough to stert pos
                    //start walk
                    StartCoroutine(WalkPathCoroutine());
                    if (debug) Debug.Log("Starting Walk Coroutine");
                }
                else
                {
                    if (debug) Debug.Log("Not close enough to start pos, need to find new path");
                    //Call a new pathfinding
                }
            }
        }
    }
    public void callerDestroy(Zombie zombie)
    {
        if (zombie == this.zombie)
        Destroy(this.gameObject);
    }
    public void OnDestroy()
    {
        
    }

    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void TimeRamaning() // Used to get the time remaining for the zombie to reach the player (will be used for later)
    {
        float PhaseTimer = zombie.PhaseTimer;
        //ZombiePhase phase = zombie.phase;
    }
    public void calculatePathDistance() //Can be used to calculate the distance of the path for the zombie to walk (will be used in a later update)
    {
        float distance = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            distance += Vector3.Distance(path[i], path[i + 1]);
        }
        Debug.Log("Total Path Distance: " + distance);
    }
}