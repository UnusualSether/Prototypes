using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPathControl : MonoBehaviour
{
    public Grid_ grid;
    public CharacterMove characterMove;
    public PathFolower pathFolower;

    private Vector3 targetPos;
    private List<Vector3> path;
    // Start is called once before the first execution
    // of Update after the MonoBehaviour is created
    
    public void pathFolowerSetUp(Grid_ grid, CharacterMove characterMove, PathFolower pathFolowerComponent)
    {
        this.grid = grid;
        this.characterMove = characterMove;
        this.pathFolower = pathFolowerComponent;
    }
    public void serchNewPathSetUp(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        if (grid.GetXYZ(targetPos, out int x,out int y,out int z))
        {
            FindPath(targetPos);
        }
        else
        {
            //throw error for now
        }
    }

    public void FindPath(Vector3 target)
    {

    }
        //path = grid.pathfinding.FindPath(transform.position, targetPos);
}