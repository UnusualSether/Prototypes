using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class CharacterMove : MonoBehaviour
{
    public Transform charTransform;
    public float speed = 2f; // Aumentei um pouco para teste, ajuste conforme necessário
    public NavMeshAgent navMesh;
    public NavMeshSurface surface;

    // Variáveis para controle de toque/swipe
    private Vector2 touchStartPos;
    private bool isTouching = false;
    // Distância mínima para considerar um swipe
    private float swipeThreshold = 50f; 

    public void Start()
    {
        charTransform = this.gameObject.GetComponent<Transform>();
        navMesh = gameObject.GetComponent<NavMeshAgent>();

        // Se for usar movimento manual, é bom pausar o destino do navmesh inicialmente
        if (navMesh != null)
        {
            navMesh.updatePosition = true; 
        }
    }

    public void Update()
    {
        //HandleMobileInput();
        ClickControls(); 
        CheckStatus();
    }

    private void HandleMobileInput()
    {
        isTouching = false;

        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            isTouching = true; 
            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
               
                Vector2 touchEndPos = touch.position;
                float deltaX = touchEndPos.x - touchStartPos.x;

                if (Mathf.Abs(deltaX) > swipeThreshold)
                {
                    if (deltaX < 0)
                        LookLeft();
                    else
                        LookRight();
                }
            }
        }
        
        else if (Input.GetMouseButton(0))
        {
            isTouching = true;
        }

        
        if (Input.GetKeyDown(KeyCode.LeftArrow)) LookLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) LookRight();

        
        if (isTouching)
        {
            WalkForwardMobile();
        }
    }

    [ContextMenu("Look Left")]
    public void LookLeft()
    {
        charTransform.Rotate(0, -90, 0);
    }

    [ContextMenu("Move Right")]
    public void LookRight()
    {
        charTransform.Rotate(0, 90, 0); 
    }

    public void WalkForwardMobile()
    {
        if (PathBlocked())
        {
            return;
        }

        Vector3 movement = transform.forward * speed * Time.deltaTime;

       
        if (navMesh != null && navMesh.enabled)
        {
            navMesh.Move(movement);
        }
        else
        {
            charTransform.position += movement;
        }
    }

    

    [Serializable]
    public class Waypoint
    {
        public Vector3 wayPointPosition;
        public int numberOfEnemies;
        public bool hasEncounter;
    }

    public List<Waypoint> wayPointList = new List<Waypoint>();

    private void ClickControls()
    {
        if (Input.GetMouseButtonDown(1)) 
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
                hasEncounter = UnityEngine.Random.Range(0, 2) == 0, 
            };
            wayPointList.Add(newWaypoint);
        }
        surface.BuildNavMesh();
    }

    public void WayPointMaintenance()
    {
        if (!NewWayPoints()) return;

        wayPointList.Clear();
        InitializeWaypoints();
    }

    [ContextMenu("GoToNextWaypoint")]
    public void FindNextWayPoint()
    {
        if (wayPointList.Count <= 0) return;

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
        if (wayPointList.Count <= 0) return;

        Vector3 inertStatus = new Vector3(0, 0, 0);

        if (GetComponent<Rigidbody>().linearVelocity != inertStatus)
        {
            encounterGate = false;
            
        }

        if (Vector3.Distance(transform.position, wayPointList[0].wayPointPosition) < 0.5f) 
        {
            if (encounterHere(wayPointList[0]) && !encounterGate)
            {
                Debug.Log("I'm in an encounter!");
                encounterGate = true; 
            }
        }
    }

    public bool encounterHere(Waypoint waypoint)
    {
        return waypoint.hasEncounter; 
    }

    public bool PathBlocked()
    {
        RaycastHit hit;
        float maxDistanceToObstacle = 0.5f;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistanceToObstacle))
        {
            
            if (!hit.collider.isTrigger)
            {
                return true;
            }
        }
        return false;
    }

    public bool NewWayPoints()
    {
        var findableWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        return findableWaypoints.Length > wayPointList.Count; 
    }
}