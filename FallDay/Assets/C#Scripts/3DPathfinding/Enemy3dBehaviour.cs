
using System.Collections;
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

    public void Zombie3SetUP(Zombie zombie, GameHandler gameHandler, GameObject player, Grid_ grid, PathFolower PathFollowerComponent) // Needs to be called by the initial spawner to pass the zombie and player references to this script
    {
        this.zombie = zombie;
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
    }
    public void afterWalk()// To Add functions and other events justt in case
    {
        if (debug) Debug.Log("Afer Walk Called");
        StartCoroutine(DamegeCorutine());

    }

    private IEnumerator DamegeCorutine()
    {
        if (debug) Debug.Log($"Damage Corutine Called");
        do
        {
            yield return new WaitForSeconds(zombie.enemyData.DamegeTimer);
            if (debug) Debug.Log($"Damage Corutine yield return called");
            gameHandler.CallPlayerDamage(zombie.enemyData.Damage);

        } while (whileControl);
    }

    public void callerDestroy(Zombie zombie)
    {
        if (activationGate && this.zombie == zombie)
        {
            Destroy(this.gameObject);
        }
    }
}