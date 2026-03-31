
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class TechDemoCamera : MonoBehaviour
{
    
    [Header("Ajustes de Camera")]
    [SerializeField] Transform _target;
    [SerializeField] float _distanceFromTarget = 0;
    [SerializeField] float _heightOffset = 2.0f;
    [SerializeField] float _initialPitch = 20.0f;
    [SerializeField]private float sensitivity = 10f;
    [SerializeField] private float _maxRotation = 60f;
    private float _yam = 0f;
    private float _pitch = 0f;


     void Start()
    {
        
        
    }
    void Update()
    {
        
        HandleInput();

        Quaternion yamRotation = Quaternion.Euler(_initialPitch, _yam, 0f);

        RotateCamera(yamRotation);
    }
    
    public void HandleInput()
    {
        Vector2 inputDelta= Vector2.zero;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputDelta = touch.deltaPosition*0.1f;

        }
        _yam += inputDelta.x * sensitivity * Time.deltaTime;
        //_pitch -= inputDelta.y * sensitivity * Time.deltaTime;
        // não precisa desse codigo se for deixar o jogador modificar a pisção da camera
        _yam = Mathf.Clamp(_yam, -_maxRotation, _maxRotation);
       // _pitch = Mathf.Clamp(_yam, -_maxRotation, _maxRotation);
        
    }

    void RotateCamera(Quaternion rotation)
    {

        Vector3 targetPositionWithHeight = _target.position + Vector3.up * _heightOffset;
        Vector3 positionOffset = rotation * new Vector3(0, 0, -_distanceFromTarget);
        transform.position = targetPositionWithHeight + positionOffset;
        transform.rotation = rotation;
        /*
        Vector3 positionOffset = rotation * new Vector3(0, 0, -_distanceFromTarget);
        transform.position = _target.position + positionOffset;
        transform.rotation = rotation;
        */
    }


    /*
    public Transform toLookAt;
    public Vector3 offset;

    
    public void LateUpdate()
    {
        transform.position = toLookAt.position + offset;
    }
    public void Update()
    {
        transform.LookAt(toLookAt.position);
        CamControls();
        if (HasCameraMoved(cameraCachedPos,Camera.main.transform.position) || HasPlayerMoved(playerCachedPos, toLookAt.transform.position))
        {
            WallDisable();
        }
    }

    private void CamControls()
    {
        float speed = 18f;
        //float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical"); 

        if (!CanCameraMoveDown(vertical))
        {
           if (vertical < 0)
            {
                vertical = 0;
            }
        }

        if (!CanCameraMoveUp(vertical))
        {
            if (vertical > 0)
            {
                vertical = 0;
            }
        }
        
        transform.Translate(Vector3.right * 0 * speed * Time.deltaTime, Space.Self);

        
        transform.Translate(Vector3.up * vertical * speed * Time.deltaTime, Space.Self);
    }

    private bool CanCameraMoveDown(float vertical)
    {

        if (vertical < 0 && Camera.main.transform.position.y < 1)
        {
            return false;
        }
        
            return true;
    }

    private bool CanCameraMoveUp(float vertical)
    {
        if (vertical > 0 && Camera.main.transform.position.y > 6)
        {
            return false;
        }

            return true;
    }


    public List<GameObject> cachedWalls = new List<GameObject>();
    public string[] hitObjectNames;
    public Vector3 cameraCachedPos;
    public Vector3 playerCachedPos;
    private void WallDisable()
    {
        
        

        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = transform.forward;
        RaycastHit[] hitObject;
        float sphereCastSize = 0.1f;

        float maxwallSeeDistance = Vector3.Distance(transform.position, toLookAt.position);


        hitObject = Physics.SphereCastAll(rayOrigin,sphereCastSize, rayDirection, maxwallSeeDistance);

        List<GameObject> HitObjectToGO = new List<GameObject>();
        
        foreach (var hitObj in hitObject)
        {
            if (hitObj.collider.gameObject.CompareTag("Wall"))
            {
                HitObjectToGO.Add(hitObj.collider.gameObject);
            }
           
        }
        

        if (cachedWalls.SequenceEqual(HitObjectToGO))
        {
            return;
        }

        if (cachedWalls.Count() <= 0)
        {
            return;
        }

        foreach(var wall in cachedWalls)
        {
            if (!HitObjectToGO.Contains(wall))
            {
                wall.GetComponent<Renderer>().forceRenderingOff = false;
                cachedWalls.Remove(wall);
            }
        }

        foreach (var foundWall in HitObjectToGO)
        {

            if (!cachedWalls.Contains(foundWall))
            {
                cachedWalls.Add(foundWall);
                foundWall.GetComponent<Renderer>().forceRenderingOff = true;
            }
            
        }


    }

    private bool HasCameraMoved(Vector3 cachedPosition, Vector3 currentPosition)
    {
        if (cachedPosition != currentPosition)
        {
            cameraCachedPos = currentPosition;
            return true;
        }

        else
        {
            return false;
        }
    }

    private bool HasPlayerMoved(Vector3 cachedPosition, Vector3 currentPosition)
    {
        if (cachedPosition != currentPosition)
        {
            playerCachedPos = currentPosition;
            return true;
        }

        else
        {
            return false;
        }
    }

    */







}
