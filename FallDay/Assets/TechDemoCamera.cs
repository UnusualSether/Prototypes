using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;

public class TechDemoCamera : MonoBehaviour
{
  
    public Transform toLookAt;
    

    

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
        float horizontal = Input.GetAxis("Horizontal");
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
        
        transform.Translate(Vector3.right * horizontal * speed * Time.deltaTime, Space.Self);

        
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








}
