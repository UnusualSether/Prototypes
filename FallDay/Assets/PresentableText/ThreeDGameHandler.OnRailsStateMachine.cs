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
        Cleanup,
        PlayerChoice

    }

    public GameObject player;
    




    [Header("RAILS STATE MACHINE")]
    [Space(10)]
    public States currentState;

    public States[] stateFlow = 
        {
        States.OnRail,
        States.Encounter,
        States.Cleanup,
        States.PlayerChoice
        };


    //State Start Events
    public static event Action RailStarted;
    public static event Action EncounterStarted;
    public static event Action CleanupStarted;
    public static event Action PlayerChoiceStarted;

    //State End Events
    public static event Action RailEnded;
    public static event Action EncounterEnded;
    public static event Action CleanupEnded;
    public static event Action PlayerChoiceEnded;

    public Dictionary<States, Action> stateToFunction;
    public Dictionary<States, Action> stateToFunctionEnds;

    partial void OnStartExtendRailMethod()
    {
        stateToFunction = new Dictionary<States, Action>
        {
            {States.OnRail, StartRails },
            {States.Encounter, StartEncounter },
            {States.Cleanup, StartCleanup },
            {States.PlayerChoice, StartPlayerChoice },

        };

        stateToFunctionEnds = new Dictionary<States, Action>

        {
            {States.OnRail, EndRails },
            {States.Encounter, EncounterEnd },
            {States.Cleanup, CleanupEnd },
            {States.PlayerChoice, EndPlayerChoice }
             

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

    }

    public void EndRails()
    {
        RailEnded?.Invoke();

        if (handler.ui.visible == true)
        {
            return;
        }

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
        
            EncounterEnded?.Invoke();
            StartCleanup();
        
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


        StartPlayerChoice();
        
    }

    public void StartPlayerChoice()
    {
        currentState = States.PlayerChoice;
        Debug.Log("Player choice started");
    }

    public void EndPlayerChoice()
    {
        StartRails();
    }

    void PlayerToEncounterGate()
    {
        if (currentState != States.OnRail)
        {
            return ;
        }

        else
        {
            EndRails();
        }
    }
   

}
