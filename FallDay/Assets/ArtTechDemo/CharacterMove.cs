using UnityEngine;

public class CharacterMove : MonoBehaviour
{

    public Transform charTransform;

    public float speed = 0.2f;
    public void Start()
    {
        charTransform = this.gameObject.GetComponent<Transform>();
    }

    public void Update()
    {
        WalkForward();
        LookLeft();
        LookRight();
    }


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
}
