using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using JetBrains.Annotations;
using System.Security.Cryptography;
using Unity.AI.Navigation;
using System.Linq;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net;

public class CharacterMove : MonoBehaviour
{


    #region Events
    public static event Action PlayerMoved;

    public static event Action PlayerHasReachedNextPoint;

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
    void OnEnable() { ThreeDGameHandler.RailStarted += FindNextWaypoint; ThreeDGameHandler.RoomSetupComplete += InitializeWaypoints;  }
    void OnDisable() { ThreeDGameHandler.RailStarted -= FindNextWaypoint; ThreeDGameHandler.RoomSetupComplete -= InitializeWaypoints; }
    

    public void Update()
    {

        
        

}


    [Serializable]
    public class Waypoint
    {
        public Vector3 wayPointPosition;

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
                wayPointPosition = item.transform.position
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
        if(wayPointList.Count <= 0)
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
        public void GoToNextWaypoint(Waypoint nextWaypoint)
    {

         navMesh.SetDestination(nextWaypoint.wayPointPosition);

        PlayerMoved?.Invoke();
    }

    bool encounterGate = false;
    private void CheckStatus()
    {

        if (wayPointList.Count <= 0)
        {
            return;
        }


        if (navMesh.remainingDistance < 0.1)
        {
            PlayerHasReachedNextPoint?.Invoke();
        }

        if (transform.position == wayPointList[0].wayPointPosition)
        {

            InitializeWaypoints();


        }
    }


   

    public bool PathBlocked()
    {
        Ray ray;
        RaycastHit hit;
        float maxDistanceToObstacle = 0.5f;

        if(Physics.Raycast(transform.position,transform.forward, out hit, maxDistanceToObstacle))
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
}
