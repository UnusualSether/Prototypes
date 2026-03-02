using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Collections;

[Serializable]
public class OnRailsStateMachine : MonoBehaviour
{
    public enum States
    {
        OnRail,
        Encounter,
        Cleanup
    }
    public States currentState;

    public States[] stateFlow = 
        {
        States.OnRail,
        States.Encounter,
        States.Cleanup
        };


    //State Start Events
    public event Action RailStarted;
    public event Action EncounterStarted;
    public event Action CleanupStarted;

    //State End Events
    public event Action RailEnded;
    public event Action EncounterEnded;
    public event Action CleanupEnded;

    public Dictionary<States, Action> stateToFunction;

    private void Start()
    {
        stateToFunction = new Dictionary<States, Action>
        {
            {States.OnRail, StartRails },
            {States.Encounter, StartEncounter },
            {States.Cleanup, StartCleanup }

        };
    }


    #region Test Purposes
    public void CheckCurrentState(States stateToCheck)
    {
        Debug.Log($"Current state is {stateToCheck}");
    }

    [ContextMenu("StartTheAutoPlay")]
    public void StartAutoPlayer()
    {
        StartCoroutine(AutoPlayer());
    }

    IEnumerator AutoPlayer()
    {
        yield return new WaitForSeconds(5);
        AdvanceStateFlow();
        StartAutoPlayer();
    }
    #endregion

    [ContextMenu("AdvanceGameState")]
    public void AdvanceStateFlow()
    {
        //Set the public States to the next state
        int currentStateIndexer = Array.IndexOf(stateFlow, currentState);
        currentStateIndexer  = (currentStateIndexer + 1) % stateFlow.Length;
        currentState = stateFlow[currentStateIndexer];

        //Activate the newly switched to state's function

        Action whichAction = stateToFunction[currentState];
        whichAction.Invoke();
       

    }


    
    public void StartRails()
    {
        Debug.Log("Now on Rails");
        RailStarted?.Invoke();

        
    }

    public void EndRails()
    {
        RailEnded?.Invoke();
    }
    
    public void StartEncounter()
    {
        Debug.Log("Now on Encounter");
        EncounterStarted?.Invoke();
    }

    public void EncounterEnd()
    {
        EncounterEnded?.Invoke();
    }


    public void StartCleanup()
    {
        Debug.Log("Now on Cleanup");
        CleanupStarted?.Invoke();
    }
       
    public void CleanupEnd()
    {
        CleanupEnded?.Invoke();
    }



}
