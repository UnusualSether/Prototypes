using System.Collections.Generic;
using UnityEngine;

public class PathFolower : MonoBehaviour
{
    private List<Vector3> path;
    public void SetNewPath(List<Vector3> newPath)
    {
        path = newPath;
    }
}
