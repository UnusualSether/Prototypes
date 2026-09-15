using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
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
        canWalk();
    }
    public void canWalk()
    {
        //is pos of curerent Object near start pos?
        //true - start path
        //false - serch new path
        if (path != null)
        {
            if (path != null && path.Count > 0) // is there path to follow
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
                    if (debug) Debug.Log("Not close enough to start pos, need to find new path");
                    //Call a new pathfinding
                    SerchNewPath();
                }
                if((gameObject.transform.position - path[path.Count - 1]).magnitude < 0.2f)
                {
                    Debug.Log($"Has gotten Close to the end of the path");
                    // Add a damage event to the player here
                }
            }
        }
    }
    public void callerDestroy(Zombie zombie) //This should be unique to the zombie
    {
        if (zombie == this.zombie)
        Destroy(this.gameObject);
    }
}