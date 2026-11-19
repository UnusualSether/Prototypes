using UnityEngine;
using BetterSceneManagement;
using System;
using System.Collections;
using UnityEngine.UI;

public enum AnimationType
{
    Door,
    Stairs
}

public class GeneralAnimator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static event Action AnimationDone;

    private void OnEnable()
    {
        BetterSceneManager.StartTransition += PlayTransitionAnimation;

        BetterSceneManager.DoneLoading += ReadyToShowScene;
    }
    private void OnDisable()
    {
        BetterSceneManager.DoneLoading -= ReadyToShowScene;

        BetterSceneManager.StartTransition -= PlayTransitionAnimation;
    }


    public Camera cam;

    public Canvas canvas;

    public Animator animator;

    public Image door1;

    public Image door2;
    

    public void PlayTransitionAnimation(AnimationType type)
    {
        if (type == AnimationType.Door)
        {
            animator.SetTrigger("CloseDoors");
        }

        if (type == AnimationType.Stairs)
        {
            animator.SetTrigger("BuildStairs");
        }
    }

    public void AnimationDoneFunc()
    {
        StartCoroutine(DelayBeforeLoadScene(1f));
    }

    public void ReadyToShowScene()
    {
        StartCoroutine(DelayBeforeSceneReveal(1));
    }

    public void EndTransitionAnimation()
    {
        animator.SetTrigger("EndAnimation");
    }

    public void ShakeDoors()
    {
        StartCoroutine(ShakeDoors(1.5f, 4f, door1, door2));
    }


    
    public IEnumerator DelayBeforeSceneReveal(int amount)
    {

        yield return new WaitForSeconds(amount);

        EndTransitionAnimation();

    }

    public IEnumerator DelayBeforeLoadScene(float amount)
    {
        yield return new WaitForSeconds(amount);

        AnimationDone?.Invoke();
    }

    public IEnumerator ShakeDoors(float dur, float magnitude, UnityEngine.UI.Image door1, UnityEngine.UI.Image door2)
    {
        var elapsed = 0f;

        var door1_start_pos =door1.transform.position;

        var door2_start_pos = door1.transform.position;

        while (dur > elapsed)
        {

            var random_x = UnityEngine.Random.Range(-magnitude, magnitude);

            var random_y = UnityEngine.Random.Range(-magnitude, magnitude);

            door1.transform.position = new Vector3(door1.transform.position.x + random_x, door1.transform.position.y + random_y, door1.transform.position.z);

            door2.transform.position = new Vector3(door2.transform.position.x + random_x, door2.transform.position.y + random_y, door2.transform.position.z);

            elapsed += Time.deltaTime;

            yield return null;
        }


        door1.transform.position = door1_start_pos;

        door2.transform.position = door2_start_pos;


    }
   
}
