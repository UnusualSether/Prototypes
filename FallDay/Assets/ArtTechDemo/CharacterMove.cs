using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Linq;


[Serializable]
public class Waypoint
{
    public Vector3 wayPointPosition;


    public GameObject belongingRoom;

}


public class CharacterMove : MonoBehaviour
{
    #region Events
    public static event Action PlayerMoved;

    public static event Action PlayerHasReachedNextPoint;

    public static event Action<List<Waypoint>> PlayerWillClearList;

    public static event Action<Waypoint> PlayerMovingTowardsUnLoadedRoom;

    #endregion


    public Transform charTransform;

    public float speed = 0.2f;

    public NavMeshAgent navMesh;

    public NavMeshSurface surface;

    public void Start()
    {
        charTransform = this.gameObject.GetComponent<Transform>();
        navMesh = gameObject.GetComponent<NavMeshAgent>();
    }

    //Subscribe the player move to next waypoint function to whenevr the gamehandler deetcts that we're suppose to be on rails/
    void OnEnable() { ThreeDGameHandler.RailStarted += FindNextWaypoint; ThreeDGameHandler.RoomSetupComplete += InitializeWaypoints; ThreeDGameHandler.PlayerSwipedOnChoice += PlayerSwipe; }
    void OnDisable() { ThreeDGameHandler.RailStarted -= FindNextWaypoint; ThreeDGameHandler.RoomSetupComplete -= InitializeWaypoints; ThreeDGameHandler.PlayerSwipedOnChoice -= PlayerSwipe; }


    public void Update()
    {


        CheckStatus();

    }


    

    public List<Waypoint> wayPointList = new List<Waypoint>();


    [ContextMenu("InitializeWayPoints")]

    public void InitializeWaypoints()
    {

        var foundObjects = GameObject.FindGameObjectsWithTag("Waypoint");




        foreach (var item in foundObjects)
        {


            var newWaypoint = new Waypoint()
            {
                wayPointPosition = item.transform.position,

                belongingRoom = item.transform.parent.gameObject
            };

            wayPointList.Add(newWaypoint);

            Destroy(item);
        }


        surface.BuildNavMesh();


    }

    public void WayPointMaintenance()
    {
        if (!NewWayPoints())
        {
            return;
        }


        InitializeWaypoints();



    }


    [ContextMenu("GoToNextWaypoint")]
    public void FindNextWaypoint()
    {
        if (wayPointList.Count <= 0)
        {
            InitializeWaypoints();
        }

        var nextWayPoint = wayPointList[0];

        if (nextWayPoint == null)
        {
            Debug.Log("Error, could not find first of nextwaypoint list.");
            FindNextWaypoint();
        }

        wayPointList.RemoveAt(0);

        GoToNextWaypoint(nextWayPoint);

        WayPointMaintenance();

    }


    GameObject cachedPlayerRoom;
    public void GoToNextWaypoint(Waypoint nextWaypoint)
    {

        navMesh.SetDestination(nextWaypoint.wayPointPosition);

        cachedPlayerRoom = nextWaypoint.belongingRoom;

        PlayerMoved?.Invoke();
    }

    bool encounterGate = false;
    private void CheckStatus()
    {
        if (wayPointList.Count <= 0) return;

        //Debug.Log($"[Player] Checking status, has path: {navMesh.hasPath}, path pending: {navMesh.pathPending}, remaining distance: {navMesh.remainingDistance}");
        bool hasPath = navMesh.hasPath || navMesh.pathPending;
        bool reachedDestination = !navMesh.pathPending
                               && navMesh.hasPath
                               && navMesh.remainingDistance < 0.1f;

        if (reachedDestination)
        {
            Debug.Log($"[Player] Reached destination at {transform.position}");
            PlayerHasReachedNextPoint?.Invoke();
            PlayerWillClearList?.Invoke(wayPointList);
            wayPointList.Clear();
            InitializeWaypoints();

        }

        if (transform.position == wayPointList[0].wayPointPosition)
        {
            Debug.Log($"[Player] Reached waypoint at {transform.position}");
            InitializeWaypoints();
        }
    }




    public bool PathBlocked()
    {
        Ray ray;
        RaycastHit hit;
        float maxDistanceToObstacle = 0.5f;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistanceToObstacle))
        {
            return true;
        }



        return false;
    }

    public bool NewWayPoints()
    {

        var findableWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        if (findableWaypoints.Length > wayPointList.Count)
        {
            return true;
        }

        return false;



    }




    
    public void PlayerSwipe(ThreeDGameHandler.SwipeDirection dir)
    {

        Debug.Log($"[Player] It seems I'm suppose to go {(ThreeDGameHandler.SwipeDirection)dir}");
        var posToGo = FetchWayPoint(dir);
        GoToNextWaypoint(posToGo);
        PlayerMovingTowardsUnLoadedRoom?.Invoke(posToGo);

    }


    public Waypoint FetchWayPoint(ThreeDGameHandler.SwipeDirection dir)
    {
           //If dir is up = Needs to return waypoint with the highest z
           //If dir is right = Needs to return the waypoint with the lowest x
           //If dir is left = Needs to return the waypoint with the highest x


        if (dir == ThreeDGameHandler.SwipeDirection.Left)
        {
           Waypoint wayPoint = wayPointList.OrderByDescending(x => x.wayPointPosition.x).LastOrDefault();
            return wayPoint;
        }

        if (dir == ThreeDGameHandler.SwipeDirection.Right)
        {
            Waypoint wayPoint = wayPointList.OrderByDescending(x => x.wayPointPosition.x).FirstOrDefault();
            return wayPoint;
        }

        if (dir == ThreeDGameHandler.SwipeDirection.Up)
        {
            Waypoint wayPoint = wayPointList.OrderByDescending(x => x.wayPointPosition.z).FirstOrDefault();
            return wayPoint;
        }

        else
        {
            Debug.Log("Function FetchWayPoint didnt recieve a valid riection and is returning a null waypoint!");
            return null;

        }

    }
}
