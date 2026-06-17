using UnityEngine;
using System;
using UnityEngine.Scripting.APIUpdating;

public partial class ThreeDGameHandler //PlayerChoice Partial Class That Contains Swipe Controls Input and decision of action (Reeds Screen Input and assignes an action to it)
{


    public enum SwipeDirection
    {
        None,
        Left, 
        Right, 
        Up 
    }


    public SwipeDirection receivedDirection = SwipeDirection.None;

    public static event Action<SwipeDirection> PlayerSwipedOnChoice; //Event Not Working corectly function CatchDirection() within Event not being called. Unknown
    public static event Action PlayerMadeDecision;

    bool PlayerChoiceGate;


    [SerializeField] private float swipe_threshold = 100f;  // Minimum distance to count as swipe

    private Vector2 touch_start_pos;
    private Vector2 touch_end_pos;
    private SwipeDirection detected_swipe = SwipeDirection.None;

    private SwipeDirection leftandrightnulling = SwipeDirection.None;  // => Unknown ask Scott? <= 

    // Moved Update To ThreeDGameHandler.cs
    // Makes more sence to keep Start and Update Together.
    /*
    void Update() //Update?
    {
        HandleSwipeInput();
    }
    */
    void HandleSwipeInput() // Takes Screen Input and selects if correct state and Generates Start_pos & End_Pos
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

    void DetectSwipe() // Detect If Swipe is not to smoll and 
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
                detected_swipe = SwipeDirection.None;  // Down not included {Maybe add necessary _RLH107}
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
        if (PlayerChoiceGate)
        {
            return;
        }

        switch (detected_swipe)
        {
            case SwipeDirection.Up:
                leftandrightnulling = SwipeDirection.Up;
                OnSwipeUp();
                break;

            case SwipeDirection.Left:
                if (leftandrightnulling == SwipeDirection.Right)
                {
                    DeGatePlayerChoice();
                    return;
                }
                
                leftandrightnulling = SwipeDirection.Left;
                OnSwipeLeft();
                break;

            case SwipeDirection.Right:

                if (leftandrightnulling == SwipeDirection.Left)
                {
                    DeGatePlayerChoice();
                    return;
                }

                leftandrightnulling = SwipeDirection.Right;
                OnSwipeRight();
                break;

            case SwipeDirection.None:
                break;
        }

        // Reset for next swipe
        detected_swipe = SwipeDirection.None;

        PlayerChoiceGate = true;

    }

    // Your game logic for each swipe direction
    void OnSwipeUp() // How Is this Not Working?
    {
        CatchDirection(SwipeDirection.Up);
        PlayerSwipedOnChoice?.Invoke(SwipeDirection.Up);
    }

    void OnSwipeLeft()
    {
        CatchDirection(SwipeDirection.Left);
        PlayerSwipedOnChoice?.Invoke(SwipeDirection.Left);
    }

    void OnSwipeRight()
    {
        CatchDirection(SwipeDirection.Right);
        PlayerSwipedOnChoice?.Invoke(SwipeDirection.Right);
    }

    // Public getter if you need it elsewhere
    public SwipeDirection GetLastSwipe()
    {
        return detected_swipe;
    }

    public void DeGatePlayerChoice()
    {
        PlayerChoiceGate = false;
    }
}





