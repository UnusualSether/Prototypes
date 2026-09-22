using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPathControl : MonoBehaviour
{
    
    private Vector3 targetPos;
    private Grid_ grid;
    private List<Vector3> path;
    // Start is called once before the first execution
    // of Update after the MonoBehaviour is created
    
    public void pathFolowerSetUp(Grid_ grid, , Action EndPathCal, PathFolower pathFolowerComponent)
    {
        this.grid = grid;

    }
    public void serchNewPath(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        path = grid.pathfinding.FindPath(transform.position, targetPos);
    }

}