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

public class CharacterMove : MonoBehaviour
{

    public Transform charTransform;

    public float speed = 0.2f;

    public NavMeshAgent navMesh;

    public NavMeshSurface surface;
    public void Start()
    {
        charTransform = this.gameObject.GetComponent<Transform>();
        navMesh = gameObject.GetComponent<NavMeshAgent>(); 
    }

    public void Update()
    {
        WalkForward();
        LookLeft();
        LookRight();
        ClickControls();
        CheckStatus();


    }


    [Serializable]
    public class Waypoint
    {
        public Vector3 wayPointPosition;

        public int numberOfEnemies;

        public bool hasEncounter;
    }

    public List<Waypoint> wayPointList = new List<Waypoint>();


    [ContextMenu("Look Left")]
    public void LookLeft()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            charTransform.Rotate(0, -90, 0);
        }
    }

    [ContextMenu("Move Right")]
    public void LookRight()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            charTransform.Rotate(0, charTransform.rotation.y + 90, 0);
        }
        
    }

    private void ClickControls()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPoint = Input.mousePosition;

            Vector3 worldPostion = Camera.main.ScreenToWorldPoint(clickPoint);

            NavMeshPath newPath = new NavMeshPath();

            

            
        }

        
    }

    [ContextMenu("InitializeWayPoints")]    
    
    public void InitializeWaypoints()
    {
         var foundObjects = GameObject.FindGameObjectsWithTag("Waypoint");

        foreach (var item in foundObjects)
        {
            

            var newWaypoint = new Waypoint()
            {
                wayPointPosition = item.transform.position,
                numberOfEnemies = UnityEngine.Random.Range(0, 5),
                hasEncounter = UnityEngine.Random.Range(0,1) == 0,
                 

            };

            wayPointList.Add(newWaypoint);
        }

        surface.BuildNavMesh();


    }

    public void WayPointMaintenance()
    {
        if (!NewWayPoints())
        {
            return;
        }

        wayPointList.Clear();

        InitializeWaypoints();
        
        
        
    }


    [ContextMenu("GoToNextWaypoint")]
        public void FindNextWayPoint()
    {
        if(wayPointList.Count <= 0)
        {
            return;
        }

        var nextWayPoint = wayPointList[0];

        if (nextWayPoint == null)
        {
            Debug.Log("Error, could not find first of nextwaypoint list.");
            return;
        }

        wayPointList.RemoveAt(0);

        GoToNextWaypoint(nextWayPoint);

    }
        public void GoToNextWaypoint(Waypoint nextWaypoint)
    {

         navMesh.SetDestination(nextWaypoint.wayPointPosition);
    }

    bool encounterGate = false;
    private void CheckStatus()
    {

        if (encounterGate)
        {
            return;
        }

        Vector3 inertStatus = new Vector3(0, 0, 0);

        if (GetComponent<Rigidbody>().linearVelocity != inertStatus )
        {
            encounterGate = false;
            Debug.Log("I'm walking and on rails!");

        }

        if (transform.position == wayPointList[0].wayPointPosition) 
        {
            if (encounterHere(wayPointList[0]) && !encounterGate)
            {
                Debug.Log("I'm in an encounter!");
            }
        }


    }


    public bool encounterHere(Waypoint waypoint)
    {
        if (waypoint.hasEncounter == true)
        {
            return true;
        }

        return false;
    }
    public void WalkForward()
    {

       

        if (Input.GetKey(KeyCode.Space))
        {

            if (PathBlocked())
            {
                return;
            }


            charTransform.transform.position += transform.forward * speed * Time.deltaTime;
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
