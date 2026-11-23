using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public EightWayAnimationCTLR eightWayAnimationCTLR;
    public PathFolower pathFolower;

    private Grid_Generator gridGen;
    private Vector3 pathTargetPos;
    private GameHandler GH;

    bool isAiming = false;

    ////////////////////////////////////
    /**/
    private bool debug = true; /**/
    ////////////////////////////////////

    private void OnDisable()
    {
        this.GH.SucessfulShot -= ShootZombie;
    }

    public void pathFolowerSetUp(Grid_Generator gridGen, Action afterWalkCall)
    {
        this.gridGen = gridGen;
        GH = this.gridGen.GetGH();
        GH.SucessfulShot += ShootZombie;
        GameHandler.PlayerTookDamage += TakeDamage;
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
    public void TakeDamage(float a)
    {
        if (debug) Debug.Log($"PlayerControl: TakeDamage called");
        eightWayAnimationCTLR.Damaged();
    }


    public void aimAtZombie(Vector3 zombiePos)
    {
        if (debug) Debug.Log("PlayerControl: aimAtZombie called");
        isAiming = false;
        pathFolower.aimIntoPosition(zombiePos, null, () => isAiming = true);
    }
    public void ShootZombie(int a)
    {
        if(isAiming)
        {
            if (debug) Debug.Log($"PlayerControl: ShootZombie isAiming = {isAiming}");
            eightWayAnimationCTLR.Attack();
        }
    }
}