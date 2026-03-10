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
    }


    [ContextMenu("Look Left")]
    public void LookLeft()
    {
       
            charTransform.Rotate(0, -90, 0);

    }

    [ContextMenu("Move Right")]
    public void LookRight()
    {
        charTransform.Rotate(0, charTransform.rotation.y + 90, 0);
    }

    public void WalkForward()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            charTransform.transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
}
