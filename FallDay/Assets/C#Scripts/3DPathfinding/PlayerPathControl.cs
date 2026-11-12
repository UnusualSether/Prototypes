using System;
using UnityEngine;

public class PlayerPathControl : MonoBehaviour
{
    private Grid_Generator gridGen;
    private CharacterMove characterMove;
    public PathFolower pathFolower;

    private Vector3 targetPos;
    private Action afterWalkCall;
    // Start is called once before the first execution
    // of Update after the MonoBehaviour is created

    ////////////////////////////////////
    /**/ private bool debug = true; /**/
    ////////////////////////////////////

    public void pathFolowerSetUp(Grid_Generator gridGen, CharacterMove characterMove, Action afterWalkCall)
    {
        this.gridGen = gridGen;
        this.characterMove = characterMove;
        this.afterWalkCall = afterWalkCall;
        pathFolower.pathFolowerSetUp(gridGen, Vector3.negativeInfinity, 0, AfterWalk);
    }
    public void serchNewPathSetUp(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        FindPath(targetPos);
    }
    private void FindPath(Vector3 target)
    {
        pathFolower.setNewTarget(target);
        pathFolower.FindPath(transform.position, targetPos);
        pathFolower.canWalk();
    }

    public void ShootZombie(Zombie zombieBeeingTargeted)
    {
        //start turn corutine in path Follower
        //Once the turn is complete, call the Shoot animation method in CharacterMove
    }

    public void AfterWalk()
    {
        afterWalkCall();
    }
}
