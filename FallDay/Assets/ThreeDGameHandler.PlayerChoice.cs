using UnityEngine;
using System.Collections.Generic;

public partial class ThreeDGameHandler
{


    public enum SwipeDirection
    {
        Left, Right, Up, None
    }


    public SwipeDirection receivedDirection = SwipeDirection.None;




    [SerializeField] private float swipe_threshold = 100f;  // Minimum distance to count as swipe

    private Vector2 touch_start_pos;
    private Vector2 touch_end_pos;
    private SwipeDirection detected_swipe = SwipeDirection.None;

    void Update()
    {
        HandleSwipeInput();
    }

    void HandleSwipeInput()
    {

        if (currentState == States.PlayerChoice)
        {
            // Touch input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touch_start_pos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    touch_end_pos = touch.position;
                    DetectSwipe();
                    ProcessSwipe();  // Continue logic here
                }
            }
        }
    }

    void DetectSwipe()
    {
        Vector2 swipe_delta = touch_end_pos - touch_start_pos;

        // Check if swipe distance meets threshold
        if (swipe_delta.magnitude < swipe_threshold)
        {
            detected_swipe = SwipeDirection.None;
            return;
        }

        // Determine direction based on which component is larger
        float horizontal = Mathf.Abs(swipe_delta.x);
        float vertical = Mathf.Abs(swipe_delta.y);

        if (vertical > horizontal)
        {
            // Vertical swipe - check if up or down
            if (swipe_delta.y > 0)
                detected_swipe = SwipeDirection.Up;
            else
                detected_swipe = SwipeDirection.None;  // Down not included
        }
        else
        {
            // Horizontal swipe - check left or right
            if (swipe_delta.x > 0)
                detected_swipe = SwipeDirection.Right;
            else
                detected_swipe = SwipeDirection.Left;
        }
    }

    void ProcessSwipe()
    {
        switch (detected_swipe)
        {
            case SwipeDirection.Up:
                Debug.Log("Swiped UP");
                OnSwipeUp();
                break;

            case SwipeDirection.Left:
                Debug.Log("Swiped LEFT");
                OnSwipeLeft();
                break;

            case SwipeDirection.Right:
                Debug.Log("Swiped RIGHT");
                OnSwipeRight();
                break;

            case SwipeDirection.None:
                Debug.Log("Not a valid swipe");
                break;
        }

        // Reset for next swipe
        detected_swipe = SwipeDirection.None;
    }

    // Your game logic for each swipe direction
    void OnSwipeUp()
    {
        Debug.Log("Go Right");
    }

    void OnSwipeLeft()
    {
        Debug.Log("Go Right");
    }

    void OnSwipeRight()
    {
        Debug.Log("Go Right");
    }

    // Public getter if you need it elsewhere
    public SwipeDirection GetLastSwipe()
    {
        return detected_swipe;
    }
}





