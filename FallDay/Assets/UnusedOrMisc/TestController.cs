using UnityEngine;

public class TestController : MonoBehaviour
{


 
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
