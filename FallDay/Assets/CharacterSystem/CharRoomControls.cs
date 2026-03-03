using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class CharRoomControls : MonoBehaviour
{
    public Vector2 startPos;
    public float minSwipeDistance;

    void Update()
    {

        if (Input.touchCount == 0)
        {
            return;
        }


        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startPos = touch.position;
            }
           
            
            if (touch.phase == TouchPhase.Ended)
            {
                var endPos = touch.position;

                var swipe = endPos - startPos;

                if (swipe.magnitude < minSwipeDistance)
                {
                    return;
                }

                if (Math.Abs(swipe.x) > Mathf.Abs(swipe.y))
                {
                    if (swipe.x > 0)
                    {
                        SwipeRight();
                    }
                    else
                    {
                        SwipeLeft();
                    }
                }

                else
                {
                    if (swipe.y < 0)
                    {
                        return;
                    }
                    else
                    {
                        SwipeUp();
                    }

                }
            }
        }
    }

    void SwipeRight()
    {
        Debug.Log("Swiped Right");
    }

    void SwipeLeft()
    {
        Debug.Log("Swiped Left");
    }

    void SwipeUp()
    {
        Debug.Log("Swiped Up");
    }


}






