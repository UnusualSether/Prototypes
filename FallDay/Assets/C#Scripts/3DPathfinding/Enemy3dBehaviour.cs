
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
// This script is attached to the 3D zombie prefab and handles its behavior in the game world
// It receives information about the zombie and the player
// can destroy itself when the zombie is killed
// Controls the pathFinding of the 3D zombie in the game world
// 

public class Enemy3dBehaviour : MonoBehaviour //!!!!!! PLACE THIS IN A NOTHER SCRIPT FILE, THIS IS A TEMPORARY PLACEHOLDER !!!!!!//
{
    public PathFolower pathFolower; // Reference to the PathFolower component attached to the 3D zombie prefab

    private Zombie zombie; // Zombie reference to subscride to the damege && death Event and ID refrence
    private GameHandler gameHandler;
    private GameObject player; // Reference to the player object // target
    private bool activationGate = false; // Activation gate to prevent null reference errors when the zombie is not yet properly initialized
    [HideInInspector]
    public bool whileControl;
    ///////////////////////////////////
    /**/
    private bool debug = true; /**/
    ///////////////////////////////////

    public enum ZombieState
    {
        Approaching,
        Attacking
    }

    public ZombieState state = ZombieState.Approaching;

    public float minimum_attack_distance = 2f;

    public void Zombie3SetUP(Zombie zombie, GameHandler gameHandler, GameObject player, Grid_ grid, PathFolower PathFollowerComponent) // Needs to be called by the initial spawner to pass the zombie and player references to this script
    {
        Vector3 t = this.transform.position; //StartPos
        Vector3 a = player.transform.position; //EndPos

        this.zombie = zombie;
        zombie.zombie_3d_space = this;
        this.gameHandler = gameHandler;
        this.player = player;
        whileControl = true;
        if (pathFolower == null) pathFolower = GetComponent<PathFolower>(); //in case the pathFolower is not set in the inspector, it will try to get it from the same gameObject

        pathFolower.pathFolowerSetUp(grid, player.transform.position, 1, afterWalk);
        activationGate = true;
    }
    public void startWalk() // This method is called by the 3DzombieGenerator when the zombie is spawned and is ready to start moving towards the player
    {
        if (activationGate)
        {
            pathFolower.FindPath();
            pathFolower.canWalk();
        }

        StopAllCoroutines();
    }

    public void Pushback3DSpace()
    {
        Vector3 player_pos = player.transform.position;

        Vector3 diff = player_pos - this.transform.position; diff.Normalize();

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
        {
            transform.position = new Vector3(transform.position.x + (transform.position.x / 2),transform.position.y, transform.position.z);
        }

        if (Mathf.Abs(diff.z) > Mathf.Abs(diff.x))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (transform.position.z / 2));
        }

        Debug.Log("Stopped coroutine");
        StopAllCoroutines();

        StartCoroutine(Stun(2));

        
    }


    public IEnumerator Stun(int stun_dur)
    {
        yield return new WaitForSeconds(stun_dur);


        state = ZombieState.Approaching;

        RemakePath();


    }

    public void RemakePath()
    {
        pathFolower.FindPath();

        pathFolower.canWalk();
    }
    
    public void afterWalk()// To Add functions and other events justt in case
    {
        if (state == ZombieState.Approaching)
        {
            if (debug) Debug.Log("Afer Walk Called");
            StartAttackPhase();
        }

    }

    public void StartAttackPhase()
    {
        state = ZombieState.Attacking;

        CallAttack();
    }

    public void CallAttack()
    {
        if (state == ZombieState.Attacking)
        {
            StartCoroutine(DamegeCorutine());
        }
    }

    public bool IsPlayerInAttackRange()
    {
        Vector3 player_pos = player.transform.position;

        Vector3 diff = player_pos - this.transform.position;

        if (diff.x > minimum_attack_distance && diff.z > minimum_attack_distance)
        {
            return false;
        }

        else
        {
            return true;
        }
    }

    private IEnumerator DamegeCorutine()
    {

        while(state == ZombieState.Attacking)
        { 



        yield return new WaitForSeconds(zombie.enemyData.DamegeTimer);
        Debug.Log($"{gameObject.name} started attack coroutine.");
        if (debug) Debug.Log($"Damage Corutine yield return called");




         if (IsPlayerInAttackRange())
         {
            zombie.DealDamage();
         }


        }
        
    }


   
    public void callerDestroy(Zombie zombie)
    {
        if (activationGate && this.zombie == zombie)
        {
            Destroy(this.gameObject);
        }
    }
}