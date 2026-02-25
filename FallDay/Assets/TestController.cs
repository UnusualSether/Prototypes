using UnityEngine;

public class TestController : MonoBehaviour
{
    [SerializeField] public OnRailsStateMachine stateMachine;

    private void Start()
    {
        stateMachine.RailStarted += OnRailsProtocol;
        stateMachine.EncounterStarted += EncounterProtocol;
        stateMachine.CleanupStarted += CleanUpProtocol;
    }

    private void OnRailsProtocol()
    {
        Debug.Log("[Test Controller] Moving the Player Forward!");
    }

    private void EncounterProtocol()
    {
        Debug.Log("[Test Controller]Stopping the Player and activating the Minigame!");
    }

    private void CleanUpProtocol()
    {
        Debug.Log("[Test Controller]Cleaning up the Minigame and getting the player ready for the next move...");
    }
}
