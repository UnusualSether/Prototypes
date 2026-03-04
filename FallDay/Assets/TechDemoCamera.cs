using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class TechDemoCamera : MonoBehaviour
{
    public Transform[] povs = new Transform[4];
    public Transform toLookAt;
    

    

    public void Update()
    {
        transform.LookAt(toLookAt.position);
        CamControls();
        WallDisable();
    }

    private void CamControls()
    {
        float speed = 18f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical"); 
        
        transform.Translate(Vector3.right * horizontal * speed * Time.deltaTime, Space.Self);

        
        transform.Translate(Vector3.up * vertical * speed * Time.deltaTime, Space.Self);
    }


    private GameObject hiddenWall;
    private void WallDisable()
    {
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = transform.forward;
        float maxDistance = 100f;
        RaycastHit hitObject;

        if (Physics.Raycast(rayOrigin,rayDirection,out hitObject, maxDistance))
        {
            

            if (hitObject.transform.gameObject.CompareTag("Wall"))
            {
                MeshRenderer renderer = hitObject.transform.GetComponent<MeshRenderer>();
                if (hiddenWall != null)
                {
                    var previousHiddenWall = hiddenWall;
                     MeshRenderer meshToActivate = previousHiddenWall.gameObject.GetComponent<MeshRenderer>();
                    meshToActivate.forceRenderingOff = false;
                }

                hiddenWall = hitObject.transform.gameObject;
                renderer.forceRenderingOff = true;
            }

            else
            {
                MeshRenderer meshToActivate = hiddenWall.gameObject.GetComponent<MeshRenderer>();
                meshToActivate.forceRenderingOff = false;
                hiddenWall = null;
            }
        }
    }
}
