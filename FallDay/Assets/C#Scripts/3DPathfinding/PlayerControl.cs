using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public EightWayAnimationCTLR eightWayAnimationCTLR;
    public PathFolower pathFolower;

    private Grid_Generator gridGen;
    private CharacterMove characterMove;
    private Vector3 pathTargetPos;
    private GameHandler GH;
    // Start is called once before the first execution
    // of Update after the MonoBehaviour is created

    ////////////////////////////////////
    /**/ private bool debug = true; /**/
    ////////////////////////////////////

    private void OnDisable()
    {
        this.GH.SucessfulShot -= ShootZombie;
    }

    public void pathFolowerSetUp(Grid_Generator gridGen, CharacterMove characterMove, Action afterWalkCall)
    {
        this.gridGen = gridGen;
        this.characterMove = characterMove;
        this.GH = this.gridGen.GetGH();
        this.GH.SucessfulShot += ShootZombie;
        pathFolower.pathFolowerSetUp(gridGen, Vector3.negativeInfinity, 0, afterWalkCall);
    }
    public void serchNewPathSetUp(Vector3 targetPos)
    {
        this.pathTargetPos = targetPos;
        FindPath(targetPos);
    }
    private void FindPath(Vector3 target)
    {
        pathFolower.setNewTarget(target);
        pathFolower.FindPath(transform.position, pathTargetPos);
        pathFolower.canWalk();
    }

    public void ShootZombie(int a)
    {
        //start turn corutine in path Follower
        //Once the turn is complete, call the Shoot animation method in CharacterMove

        eightWayAnimationCTLR.Attack();
    }
}
