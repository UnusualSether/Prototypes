using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Collections;
using Unity.VisualScripting;


public partial class ThreeDGameHandler
{
    

    
    public enum States
    {
        OnRail,
        Encounter,
        Cleanup
    }

    

    [Header("RAILS STATE MACHINE")]
    [Space(10)]
    public States currentState;

    public States[] stateFlow = 
        {
        States.OnRail,
        States.Encounter,
        States.Cleanup
        };


    //State Start Events
    public static event Action RailStarted;
    public static event Action EncounterStarted;
    public static event Action CleanupStarted;

    //State End Events
    public static event Action RailEnded;
    public static event Action EncounterEnded;
    public static event Action CleanupEnded;

    public Dictionary<States, Action> stateToFunction;
    public Dictionary<States, Action> stateToFunctionEnds;

    partial void OnStartExtendRailMethod()
    {
        stateToFunction = new Dictionary<States, Action>
        {
            {States.OnRail, StartRails },
            {States.Encounter, StartEncounter },
            {States.Cleanup, StartCleanup }

        };

        stateToFunctionEnds = new Dictionary<States, Action>

        {
            {States.OnRail, EndRails },
            {States.Encounter, EncounterEnd },
            {States.Cleanup, CleanupEnd }


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
        States endingState = currentState;
        int currentStateIndexer = Array.IndexOf(stateFlow, currentState);
        currentStateIndexer  = (currentStateIndexer + 1) % stateFlow.Length;
        currentState = stateFlow[currentStateIndexer];

        //Activate the newly switched to state's function

        Action whichAction = stateToFunction[currentState];
        whichAction?.Invoke();

        Action whichActioEnd = stateToFunctionEnds[endingState];
        whichActioEnd?.Invoke();
       

    }


    
    public void StartRails()
    {
        Debug.Log("Now on Rails");
        RailStarted?.Invoke();
        currentState = States.OnRail;

        EndRails();

    }

    public void EndRails()
    {
        RailEnded?.Invoke();

        StartEncounter();
    }
    
    public void StartEncounter()
    {
        currentState = States.Encounter;
        Debug.Log("Now on Encounter");
        EncounterStarted?.Invoke();
    }

    public void EncounterEnd()
    {
        if (currentState == States.Encounter)
        {
            EncounterEnded?.Invoke();
            StartCleanup();
        }
        
    }


    public void StartCleanup()
    {
        currentState = States.Cleanup;

        CleanupStarted?.Invoke();

        CleanupEnd();
    }
       
    public void CleanupEnd()
    {
        CleanupEnded?.Invoke();

       
        StartRails();
        
    }



}
