using System.Collections.Generic;
using UnityEngine;

public class PathFolower : MonoBehaviour
{
    private List<Vector3> path;
    bool isPathAtEnd;
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

            }
            else
            {
                //Call a new pathfinding
            }
        }
    }
    public void walkPath()
    {
        //while()
    }
}