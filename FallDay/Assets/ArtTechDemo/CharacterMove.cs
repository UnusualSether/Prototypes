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
    private float swipeThreshold = 50f; // Distância mínima para considerar um swipe

    public void Start()
    {
        charTransform = this.gameObject.GetComponent<Transform>();
        navMesh = gameObject.GetComponent<NavMeshAgent>();

        // Se for usar movimento manual, é bom pausar o destino do navmesh inicialmente
        if (navMesh != null)
        {
            navMesh.updatePosition = true; // Nós moveremos o transform, o navmesh segue
        }
    }

    public void Update()
    {
        HandleMobileInput();
        ClickControls(); // Mantido o seu método original
        CheckStatus();
    }

    private void HandleMobileInput()
    {
        isTouching = false;

        // 1. Detecta Input de Celular (Touch)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            isTouching = true; // Dedo está na tela, vai andar

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // Verifica Swipe para Rotação ao soltar o dedo
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
        // 2. Detecta Input no PC (Clique longo do Mouse) para testes no Editor
        else if (Input.GetMouseButton(0))
        {
            isTouching = true;
        }

        // Detecta setas no PC para testar a rotação sem precisar simular swipe
        if (Input.GetKeyDown(KeyCode.LeftArrow)) LookLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) LookRight();

        // 3. Executa a caminhada
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
        charTransform.Rotate(0, 90, 0); // Corrigido para somar 90 graus a partir de 0 local
    }

    public void WalkForwardMobile()
    {
        if (PathBlocked())
        {
            return;
        }

        Vector3 movement = transform.forward * speed * Time.deltaTime;

        // Move usando NavMeshAgent para respeitar colisão, se ativado
        if (navMesh != null && navMesh.enabled)
        {
            navMesh.Move(movement);
        }
        else
        {
            charTransform.position += movement;
        }
    }

    // --- RESTANTE DO SEU CÓDIGO ORIGINAL MANTIDO INTACTO ABAIXO ---

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
        if (Input.GetMouseButtonDown(1)) // Mudei para botão direito (1) para não conflitar com o "segurar para andar" (0)
        {
            Vector3 clickPoint = Input.mousePosition;
            Vector3 worldPostion = Camera.main.ScreenToWorldPoint(clickPoint);
            NavMeshPath newPath = new NavMeshPath();
            // Lógica de pathfinding manual aqui...
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
                hasEncounter = UnityEngine.Random.Range(0, 2) == 0, // Ajustado para dar 50% de chance (0 ou 1)
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
            // Debug.Log("I'm walking and on rails!");
        }

        if (Vector3.Distance(transform.position, wayPointList[0].wayPointPosition) < 0.5f) // Melhor usar distância que igualdade exata
        {
            if (encounterHere(wayPointList[0]) && !encounterGate)
            {
                Debug.Log("I'm in an encounter!");
                encounterGate = true; // Evita que floode o console
            }
        }
    }

    public bool encounterHere(Waypoint waypoint)
    {
        return waypoint.hasEncounter; // Simplificado
    }

    public bool PathBlocked()
    {
        RaycastHit hit;
        float maxDistanceToObstacle = 0.5f;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistanceToObstacle))
        {
            // Opcional: Verificar se o objeto atingido não é um trigger ou o chão
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
        return findableWaypoints.Length > wayPointList.Count; // Simplificado
    }
}