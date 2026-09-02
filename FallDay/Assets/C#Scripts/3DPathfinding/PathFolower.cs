using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFolower : MonoBehaviour
{
    private List<Vector3> path;
    // Most Simple movement along the path
    // No Control over the speed nor how long it takes to reach the next point 
    private IEnumerator WalkPathCoroutine()
    {
        foreach (Vector3 point in path)
        {
            do
            {
                transform.position = Vector3.MoveTowards(transform.position, point, 0.1f);
                yield return new WaitForFixedUpdate();
            } while ((gameObject.transform.position - point).magnitude > 0.2f);
        }
    }
    public void SetNewPath(List<Vector3> newPath)
    {
        path = newPath;
    }
    public void canWalk()
    {
        //is pos of curerent Object near start pos?
        //true - start path
        //false - serch new path
        if (path != null && path.Count > 0)
        {
            if ((gameObject.transform.position - path[0]).magnitude < 0.2f)
            {
                //is close enoth
                //start walk
                StartCoroutine(WalkPathCoroutine());
            }
            else
            {
                //Call a new pathfinding
            }
        }
    }
}