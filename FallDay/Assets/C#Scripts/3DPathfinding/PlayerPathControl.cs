using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPathControl : MonoBehaviour
{
    private Grid_ grid;
    private CharacterMove characterMove;
    public PathFolower pathFolower;

    private Vector3 targetPos;
    private List<Vector3> path;
    private Action afterWalkCall;
    // Start is called once before the first execution
    // of Update after the MonoBehaviour is created

    /////////////////////////////////////
    /**/
    private bool debug = false; /**/
    /////////////////////////////////////

    public void pathFolowerSetUp(Grid_ grid, CharacterMove characterMove, Action afterWalkCall)
    {
        this.grid = grid;
        this.characterMove = characterMove;
        this.afterWalkCall = afterWalkCall;
        pathFolower.pathFolowerSetUp(grid, Vector3.negativeInfinity, 0, AfterWalk);
    }
    public void serchNewPathSetUp(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        //bool sa = grid.GetXYZ(targetPos, out int x, out int y, out int z);
        FindPath(targetPos);
    }
    public void FindPath(Vector3 target)
    {
        pathFolower.setNewTarget(target);
        //grid.pathfinding.FindPath(transform.position, targetPos);
        pathFolower.FindPath(transform.position, targetPos);
        pathFolower.canWalk();
    }

    public void AfterWalk()
    {
        afterWalkCall();
    }
}
